using AzertaNodes.Application.DTOs;
using AzertaNodes.Application.Interfaces;
using AzertaNodes.Core.Entities;
using AzertaNodes.Core.Entities.Database;

namespace AzertaNodes.Application.Implementations
{
    public class GraphValidationService(INodeFactory nodeFactory, IContextService contextService) : IGraphValidationService
    {
        public Result<bool> ValidateGraph(Project project, IEnumerable<NodeDto> nodes, IEnumerable<Edge> edges)
        {
            var context = new GraphContext();
            var userErrors = new List<string>();
            var nodeList = nodes.ToList();
            var edgeList = edges.ToList();

            if (nodeList.Count == 0)
            {
                return Result<bool>.Success(true);
            }

            var nodesDict = nodeList.ToDictionary(n => n.Id);
            var adjacencyList = nodeList.ToDictionary(n => n.Id, _ => new List<Guid>());
            var inDegree = nodeList.ToDictionary(n => n.Id, _ => 0);

            var incomingEdgesByTarget = edgeList.ToLookup(e => e.TargetNodeId);

            foreach (var edge in edgeList)
            {
                if (!nodesDict.ContainsKey(edge.SourceNodeId) || !nodesDict.ContainsKey(edge.TargetNodeId))
                {
                    userErrors.Add($"Invalid edge: {edge.SourceNodeId} -> {edge.TargetNodeId}");
                    continue;
                }

                adjacencyList[edge.SourceNodeId].Add(edge.TargetNodeId);
                inDegree[edge.TargetNodeId]++;
            }

            if (userErrors.Count > 0)
            {
                return CreateFailureResult(userErrors);
            }

            var queue = new Queue<NodeDto>(nodeList.Where(n => inDegree[n.Id] == 0));
            int processedNodesCount = 0;

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                processedNodesCount++;

                ValidateSingleNode(current, incomingEdgesByTarget, nodeFactory, userErrors, context, project);

                foreach (var targetId in adjacencyList[current.Id])
                {
                    inDegree[targetId]--;
                    if (inDegree[targetId] == 0)
                    {
                        queue.Enqueue(nodesDict[targetId]);
                    }
                }
            }

            if (processedNodesCount != nodeList.Count)
            {
                userErrors.Add("Graph contains a cycle and cannot be executed.");
            }
            /*var hasTriggerNode = nodeList.Any(n => n.IsTrigger);
            if (!hasTriggerNode)
            {
                userErrors.Add("Graph must contain at least one trigger node.");
            }*/

            if (userErrors.Count > 0)
            {
                return CreateFailureResult(userErrors);
            }

            return Result<bool>.Success(true);
        }

        private void ValidateSingleNode(
            NodeDto nodeDto,
            ILookup<Guid, Edge> incomingEdgesByTarget,
            INodeFactory factory,
            List<string> errors,
            GraphContext context,
            Project project)
        {
            var nodeInstance = factory.Create(nodeDto.Type, project);
            var inputs = contextService.GetInputs(context, nodeDto, incomingEdgesByTarget);

            var testResults = nodeInstance.TestExecute(inputs);
            if (testResults.IsFailure)
            {
                errors.Add($"Node '{nodeDto.Name}' (ID: {nodeDto.Id}) failed validation: {testResults.Error?.UserErrorMessage}");
            }
            else
            {
                var resultValue = testResults.Value!;
                if (resultValue.Count > 0)
                {
                    foreach (var kvp in resultValue)
                    {
                        contextService.SaveOutputs(context, nodeDto.Id, kvp.Key, kvp.Value);
                    }
                }
            }
        }

        private static Result<bool> CreateFailureResult(IEnumerable<string> errors)
        {
            return Result<bool>.Failure(new Error
            {
                ErrorType = ErrorType.Validation,
                UserErrorMessage = string.Join(Environment.NewLine, errors)
            });
        }
    }
}