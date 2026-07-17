using BenchmarkDotNet.Attributes;
using Wms.Workflow.Domain.Aggregates;
using Wms.Workflow.Domain.Enums;

namespace Wms.PerformanceTests.Benchmarks;

[MemoryDiagnoser]
public class ApprovalFlowBenchmark
{
    [Params(100, 1000)]
    public int IterationCount { get; set; }

    private ApprovalFlow _flow = null!;
    private readonly Guid _submitUserId = Guid.NewGuid();
    private readonly Guid _approverUserId = Guid.NewGuid();

    [GlobalSetup]
    public void Setup()
    {
        _flow = new ApprovalFlow(
            Guid.NewGuid(),
            "OutboundApproval",
            ApprovalFlowType.Transfer,
            description: "Performance test approval flow");

        // Build a realistic approval chain: 3 nodes
        _flow.AddNode(
            nodeName: "WarehouseManager",
            nodeType: ApprovalNodeType.Approval,
            approverRole: "WarehouseManager",
            order: 1,
            isRequired: true);

        _flow.AddNode(
            nodeName: "FinanceReview",
            nodeType: ApprovalNodeType.Approval,
            approverRole: "FinanceManager",
            conditionExpression: "Amount > 10000",
            order: 2,
            isRequired: true);

        _flow.Activate();
    }

    [Benchmark]
    public void Flow_AddNode()
    {
        for (int i = 0; i < IterationCount; i++)
        {
            if (_flow.Nodes.Count > 20)
            {
                _flow.RemoveNode(_flow.Nodes[0].Id);
            }

            _flow.AddNode(
                nodeName: $"Node-{i}",
                nodeType: ApprovalNodeType.Approval,
                approverRole: "Tester",
                order: _flow.Nodes.Count + 1);
        }
    }

    [Benchmark]
    public void ApprovalInstance_SimpleApprove()
    {
        for (int i = 0; i < IterationCount; i++)
        {
            var instance = new ApprovalInstance(
                Guid.NewGuid(),
                _flow.Id,
                _flow.FlowName,
                Guid.NewGuid(),
                "Outbound",
                $"OUT-{DateTime.Now:yyyyMMdd}-{i:D6}",
                _submitUserId,
                "TestSubmitter");

            instance.Approve(_approverUserId, comment: "Approved");
        }
    }

    [Benchmark]
    public void ApprovalInstance_Reject()
    {
        for (int i = 0; i < IterationCount; i++)
        {
            var instance = new ApprovalInstance(
                Guid.NewGuid(),
                _flow.Id,
                _flow.FlowName,
                Guid.NewGuid(),
                "Outbound",
                $"OUT-{DateTime.Now:yyyyMMdd}-{i:D6}",
                _submitUserId,
                "TestSubmitter");

            instance.Reject(_approverUserId, comment: "Rejected - missing documentation");
        }
    }

    [Benchmark]
    public void ApprovalInstance_ResubmitCycle()
    {
        for (int i = 0; i < IterationCount; i++)
        {
            var instance = new ApprovalInstance(
                Guid.NewGuid(),
                _flow.Id,
                _flow.FlowName,
                Guid.NewGuid(),
                "Outbound",
                $"OUT-{DateTime.Now:yyyyMMdd}-{i:D6}",
                _submitUserId,
                "TestSubmitter");

            instance.Reject(_approverUserId, comment: "Need more info");
            instance.Resubmit(comment: "Added details");
            instance.Approve(_approverUserId, comment: "Now approved");
        }
    }

    [Benchmark]
    public void ApprovalFlow_CreateAndActivate()
    {
        for (int i = 0; i < IterationCount; i++)
        {
            var flow = new ApprovalFlow(
                Guid.NewGuid(),
                $"Flow-{i}",
                ApprovalFlowType.Transfer,
                description: $"Benchmark flow {i}");

            flow.AddNode(
                nodeName: "Manager",
                nodeType: ApprovalNodeType.Approval,
                approverRole: "Manager",
                order: 1,
                isRequired: true);

            flow.Activate();
        }
    }
}
