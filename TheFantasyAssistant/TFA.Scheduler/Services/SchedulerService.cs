using Microsoft.Azure.WebJobs;
using System.Threading.Tasks;
using System.Threading;
using TFA.Scheduler.Data;

namespace TFA.Scheduler.Services;

public class SchedulerService
{
    private readonly IRequestService _requestService;

    public SchedulerService(IRequestService requestService)
    {
        _requestService = requestService;
    }

    [FunctionName(nameof(Every30Minutes))]
    public async Task Every30Minutes([TimerTrigger(RunTimes.Every30Minutes)] TimerInfo timer, CancellationToken cancellationToken)
    {
        await _requestService.HandleScheduledRequests(RunTime.Every30Minutes, cancellationToken);
    }

    [FunctionName(nameof(EveryHour))]
    public async Task EveryHour([TimerTrigger(RunTimes.EveryHour)] TimerInfo timer, CancellationToken cancellationToken)
    {
        await _requestService.HandleScheduledRequests(RunTime.EveryHour, cancellationToken);
    }

    [FunctionName(nameof(Every3Hours))]
    public async Task Every3Hours([TimerTrigger(RunTimes.Every3Hours)] TimerInfo timer, CancellationToken cancellationToken)
    {
        await _requestService.HandleScheduledRequests(RunTime.Every3Hours, cancellationToken);
    }

    [FunctionName(nameof(Every6Hours))]
    public async Task Every6Hours([TimerTrigger(RunTimes.Every6Hours)] TimerInfo timer, CancellationToken cancellationToken)
    {
        await _requestService.HandleScheduledRequests(RunTime.Every6Hours, cancellationToken);
    }

    [FunctionName(nameof(PM7))]
    public async Task PM7([TimerTrigger(RunTimes.PM7)] TimerInfo timer, CancellationToken cancellationToken)
    {
        await _requestService.HandleScheduledRequests(RunTime.PM7, cancellationToken);
    }

    [FunctionName(nameof(PM810))]
    public async Task PM810([TimerTrigger(RunTimes.PM810)] TimerInfo timer, CancellationToken cancellationToken)
    {
        await _requestService.HandleScheduledRequests(RunTime.PM810, cancellationToken);
    }

    [FunctionName(nameof(PM9))]
    public async Task PM9([TimerTrigger(RunTimes.PM9)] TimerInfo timer, CancellationToken cancellationToken)
    {
        await _requestService.HandleScheduledRequests(RunTime.PM9, cancellationToken);
    }

    [FunctionName(nameof(Every12Hours))]
    public async Task Every12Hours([TimerTrigger(RunTimes.Every12Hours)] TimerInfo timer, CancellationToken cancellationToken)
    {
        await _requestService.HandleScheduledRequests(RunTime.Every12Hours, cancellationToken);
    }

    [FunctionName(nameof(Every10Minutes))]
    public async Task Every10Minutes([TimerTrigger(RunTimes.Every10Minutes)] TimerInfo timer, CancellationToken cancellationToken)
    {
        await _requestService.HandleScheduledRequests(RunTime.Every10Minutes, cancellationToken);
    }
}
