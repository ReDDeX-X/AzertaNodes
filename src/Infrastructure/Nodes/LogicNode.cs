using AzertaNodes.Core.Entities;
using AzertaNodes.Core.Entities.Node;
using NCalc;

namespace AzertaNodes.Infrastructure.Nodes
{
    public class LogicNode : BaseNode
    {
        public override string Name { get; } = "Logic Node";

        public override async ValueTask<Dictionary<string, NodeExecutionResult>> ExecuteAsync(Dictionary<string, NodeExecutionResult> inputs, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            bool condition;

            if (inputs.TryGetValue(NodeConstants.ExpressionKey, out var exprResult))
            {
                var expressionString = exprResult.GetValue<string>();
                if (string.IsNullOrWhiteSpace(expressionString))
                {
                    throw new ArgumentException("Expression cannot be empty.");
                }
                condition = EvaluateExpression(expressionString, inputs, cancellationToken);
            }
            else if (inputs.TryGetValue(NodeConstants.BooleanKey, out var boolResult))
            {
                condition = boolResult.GetValue<bool>();
            }
            else
            {
                throw new KeyNotFoundException($"Provide either {NodeConstants.ExpressionKey} or {NodeConstants.BooleanKey}.");
            }
            var result = new Dictionary<string, NodeExecutionResult>();

            return new Dictionary<string, NodeExecutionResult>
            {
                [NodeConstants.TruePathKey] = condition ? NodeExecutionResult.CreateNullResult() : NodeExecutionResult.Skipped(),
                [NodeConstants.FalsePathKey] = condition ? NodeExecutionResult.Skipped() : NodeExecutionResult.CreateNullResult()
            };
        }
        public override Result<Dictionary<string, NodeExecutionResult>> TestExecute(Dictionary<string, NodeExecutionResult> inputs)
        {
            try
            {
                if (inputs.TryGetValue(NodeConstants.ExpressionKey, out var exprResult))
                {
                    var expressionString = exprResult.GetValue<string>();
                    if (string.IsNullOrWhiteSpace(expressionString))
                    {
                        return Result<Dictionary<string, NodeExecutionResult>>.Failure(new Error() { ErrorType = ErrorType.Validation, UserErrorMessage = "Expression cannot be empty." });
                    }
                    var expr = new Expression(expressionString);
                    if (expr.HasErrors()) return Result<Dictionary<string, NodeExecutionResult>>.Failure(new Error() { UserErrorMessage = expr.Error.Message, ErrorType = ErrorType.Validation });
                }
                else if (!inputs.ContainsKey(NodeConstants.BooleanKey))
                {
                    throw new KeyNotFoundException($"Provide either {NodeConstants.ExpressionKey} or {NodeConstants.BooleanKey}.");
                }

                return Result<Dictionary<string, NodeExecutionResult>>.Success([]);
            }
            catch (Exception ex)
            {
                return Result<Dictionary<string, NodeExecutionResult>>.Failure(new Error() { ErrorType = ErrorType.Failure, UserErrorMessage = ex.Message });
            }
        }
        private static bool EvaluateExpression(string expressionString, Dictionary<string, NodeExecutionResult> inputs, CancellationToken cancellationToken)
        {
            var expression = new Expression(expressionString);
            foreach (var input in inputs)
            {
                expression.Parameters[input.Key] = input.Value.Value;
            }
            var rawResult = expression.Evaluate(cancellationToken);
            if (rawResult is bool result) return result;

            try
            {
                return Convert.ToBoolean(rawResult);
            }
            catch
            {
                throw new InvalidCastException($"Expression evaluated to {rawResult?.GetType().Name}, but boolean was expected.");
            }
        }
    }
}