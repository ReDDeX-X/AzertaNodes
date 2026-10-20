using AzertaNodes.Application.DTOs;
using AzertaNodes.Application.Interfaces;
using AzertaNodes.Core.Entities;
using AzertaNodes.Core.Entities.Database;

namespace AzertaNodes.Application.Implementations
{
    public class DirectedAcyclicGraph(INodeFactory nodeFactory, IContextService contextService) : IGraphRunner
    {
        private class NodeState(int initialInDegree)
        {
            public int InDegree = initialInDegree;
        }

        public async Task<Dictionary<string, object>> RunAsync(Project project, List<NodeDto> nodes, IEnumerable<Edge> edges, CancellationToken cancellationToken = default)
        {
            if (nodes is null || nodes.Count == 0)
            {
                return [];
            }

            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            var token = cts.Token;
            var context = new GraphContext();
            var graphCompletionSource = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

            var nodesDict = nodes.ToDictionary(n => n.Id);
            var edgesBySource = edges.ToLookup(e => e.SourceNodeId);
            var edgesByTarget = edges.ToLookup(e => e.TargetNodeId);

            var nodeStates = nodes.ToDictionary(n => n.Id, n => new NodeState(0));
            foreach (var edge in edges)
            {
                nodeStates[edge.TargetNodeId].InDegree++;
            }

            int remainingNodes = nodes.Count;

            var startNodes = nodes.Where(n => nodeStates[n.Id].InDegree == 0).ToList();
            foreach (var startNode in startNodes)
            {
                _ = Task.Run(() => ExecuteNodeAsync(startNode.Id), token);
            }

            await graphCompletionSource.Task;

            return contextService.GetFinalOutputs(context);

            async Task ExecuteNodeAsync(Guid nodeId)
            {
                var nodeDto = nodesDict[nodeId];
                bool executionFailed = false;

                try
                {
                    if (nodeDto.IsSync) token.ThrowIfCancellationRequested();
                    await ProcessNodeAsync(nodeDto);
                }
                catch (OperationCanceledException)
                {
                    executionFailed = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    executionFailed = true;
                    HandleNodeFailure(nodeDto, ex);
                }
                finally
                {
                    FinalizeNodeExecution(nodeId, executionFailed);
                }
            }

            async Task ProcessNodeAsync(NodeDto nodeDto)
            {

                var nodeLogic = nodeFactory.Create(nodeDto.Type, project);
                var inputs = contextService.GetInputs(context, nodeDto, edgesByTarget);

                bool isSkipExecution = nodeDto.IsWaitAll
                    ? inputs.Values.Any(value => value.IsSkipped)
                    : inputs.Values.All(value => value.IsSkipped);

                if (isSkipExecution)
                {
                    contextService.PropagateSkip(context, nodeDto.Id, edgesBySource);
                    return;
                }

                var results = await nodeLogic.ExecuteAsync(inputs, token);

                if (results is null)
                {
                    return;
                }

                foreach (var keyValuePair in results)
                {
                    contextService.SaveOutputs(context, nodeDto.Id, keyValuePair.Key, keyValuePair.Value);

                    if (nodeDto.IsOutput)
                    {
                        contextService.SaveFinalOutputs(context, keyValuePair.Key, keyValuePair.Value);
                    }
                }
            }

            void HandleNodeFailure(NodeDto nodeDto, Exception ex)
            {
                if (nodeDto.StopGraphOnFailure)
                {
                    graphCompletionSource.TrySetException(ex);
                    cts.Cancel();
                }
            }

            void FinalizeNodeExecution(Guid nodeId, bool executionFailed)
            {
                bool shouldTriggerChildren = !executionFailed && !token.IsCancellationRequested;

                foreach (var edge in edgesBySource[nodeId])
                {
                    var childId = edge.TargetNodeId;
                    int newInDegree = Interlocked.Decrement(ref nodeStates[childId].InDegree);

                    if (newInDegree == 0)
                    {
                        if (shouldTriggerChildren)
                        {
                            _ = Task.Run(() => ExecuteNodeAsync(childId), token);
                        }
                        else
                        {
                            AbortNodeTree(childId);
                        }
                    }
                }

                if (Interlocked.Decrement(ref remainingNodes) == 0)
                {
                    graphCompletionSource.TrySetResult();
                }
            }

            void AbortNodeTree(Guid startNodeId)
            {
                var queue = new Queue<Guid>();
                queue.Enqueue(startNodeId);

                while (queue.Count > 0)
                {
                    var currentId = queue.Dequeue();
                    contextService.PropagateSkip(context, currentId, edgesBySource);

                    if (Interlocked.Decrement(ref remainingNodes) == 0)
                    {
                        graphCompletionSource.TrySetResult();
                    }

                    foreach (var edge in edgesBySource[currentId])
                    {
                        var childId = edge.TargetNodeId;
                        int newInDegree = Interlocked.Decrement(ref nodeStates[childId].InDegree);

                        if (newInDegree == 0)
                        {
                            queue.Enqueue(childId);
                        }
                    }
                }
            }
        }
    }
}