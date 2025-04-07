using System.Collections.Concurrent;

namespace compete_poco.Infrastructure.Services
{
    public class ActionTimeScheduler : IActionTimeScheduler
    {
        private static readonly ConcurrentDictionary<int, TimeSchedulerActionBag> _timers = new();


        public void RefreshNotifyingAboutTime(ActionTimeSchedulerParameters p)
        {
            if (!_timers.TryGetValue(p.Id, out var timerBag))
                throw new InvalidOperationException("Такого ключа не существует");
            timerBag.Mutex.WaitOne();
            timerBag.Parameters.Input = p.Input;
            timerBag.Parameters.AvailableSeconds = p.AvailableSeconds;
            timerBag.Mutex.ReleaseMutex();
        }
        public async Task StartNotifyAboutTime(ActionTimeSchedulerParameters p)
        {  
            var cts = new CancellationTokenSource();
            var mutex = new Mutex();
            var timerBag = new TimeSchedulerActionBag()
            {
                Parameters = p,
                CancellationTokenSource = cts,
                Mutex = mutex
            };
            if (!_timers.TryAdd(p.Id, timerBag))
                throw new ArgumentException("Ключ таймера уже заполнен");
            var timerTask = Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(1000);
                    if (!_timers.TryGetValue(p.Id, out var timerBagBackup))
                        throw new InvalidOperationException("Возможно другой поток уже удалил таймер для этого ключа");
                    mutex.WaitOne();
                    timerBagBackup.Parameters.AvailableSeconds--;
                    mutex.ReleaseMutex();
                    await timerBagBackup.Parameters.EverysecondAction(timerBagBackup.Parameters.AvailableSeconds);
                    if (timerBagBackup.Parameters.AvailableSeconds.Equals(0) && timerBagBackup.Parameters.Action is not null)
                        await timerBagBackup.Parameters.Action(timerBagBackup.Parameters.Input);
                    if (timerBagBackup.CancellationTokenSource.IsCancellationRequested)
                        break;
                }
            }, timerBag.CancellationTokenSource.Token);
            timerBag.CurrentTimer = timerTask;
            await timerTask;
        }

        public async Task StopNotifyingAboutTime(int id)
        {
            if (!_timers.TryGetValue(id, out var timerBag))
                throw new InvalidOperationException("Возможно таймер для ключа уже был завершен");
            timerBag.CancellationTokenSource.Cancel();
            try
            {
                await timerBag.CurrentTimer;
            }
            catch
            {
                throw new InvalidOperationException("Исключение которые происходят при вызове функции на 0 отметке таймера, " +
                    "должны быть обработаны самой функцией" +
                    "Если у вас все равно произошло исключение, возможно, ваш код не потокобезопасный");
            }

            finally
            {
                timerBag.CancellationTokenSource.Dispose();
                if (!_timers.TryRemove(id, out timerBag))
                    throw new InvalidOperationException("Нужно очищать только активные таймеры, Вам следует следить за их вызовами!");
            }
        }
    }
}
