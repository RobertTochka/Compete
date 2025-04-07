using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Compete_POCO_Models.Infrastrcuture.Data
{
    public static class AppDictionary
    {
        public static string MessageIsEmpty => "Сообщение не может быть пустым";

        public static string All => nameof(All);

        public static string Month => nameof(Month);
        public static string Day => nameof(Day);
        public static string Week => nameof(Week);
        public static int MaxRetryCount => 3;
        public static string IntervalRequired => "Интервал является обязательным полем";
        public static string StateIsRequired => "Поле состояния является обязательным";
        public static string LabelNotValid => "Метка оплаты недействительна";
        public static string NotExistingAlready => "Сущность уже удалена";
        public static string UserNotExisting => "Такого пользователя не существует";
        public static string RetryExceeded => "Превышено максимальное кол-во попыток транзакции";
        public static string ServerErrorOcurred => "Произошла ошибка на сервере";
        public static string ConcurrencyUpdateError => "Приложение обновилось. Попробуйте еще";
        public static string LobbyWasResetted => "Лобби было перезапущено из-за ошибки на сервере";
        public static string GameServerUnderHighDemand => "Сервер загружен. Выберите другой";
        public static string GameStateNotFull => "Игровое состояние не является полным";
        public static string GameServerNotFound => "Сервер для обновления игрового состояния не найден";
        public static string ChatNotExist => "Такого чата не существует";
        public static string PermissionDenied => "У вас нет прав на это действие";
        public static string DeniedMap => "Нельзя выбрать эту карту";
        public static string NotYourStep => "Сейчас не ваша очередь";
        public static string MapsForVetoDifferentAmount => "Вето должно состоять из 7 карт";
        public static string TeamsNotFilled => "Одна из команд не заполнена";
        public static string EnemyIsEmpty => "Нет участников в противоположной команде";
        public static string TeamBidsBigDifference => "Ставки команд не могут отличаться больше, чем на 10%";
        public static string EmptyTeam => "У вас нет команды или прав на ее изменение";
        public static string TeamNameIsNotValid => "Укажите корректное имя команды";
        public static string NotEnoughMoney => "На вашем счету нет должной суммы";
        public static string ThanksForTopUp => "Благодарим за доверие к сервису!";
        public static string ConfirmOnPhone => "Уведомление о подтверждении отправлено на телефон!";
        public static string NotSupportedPayment => "Такой метод не поддерживается";
        public static string OnlyCreatorCanEdit => "Только создатель лобби может изменять конфигурацию";
        public static string UserNotFound => "Такого пользователя не существует";
        public static string JoinLobbyNotFound => "Возможно, это лобби уже было расформировано";
        public static string UserAlreadyInLobby => "Вы не можете присоединиться к нескольким лобби сразу";
        public static string LobbyAlreadyFull => "Все команды уже укомплектованы";
        public static string TeamAlreadyFull => "Команда уже собрана";
        public static string InviteHasExpired => "Не удалось подтвердить приглашение. Возможно, его действие закончилось";
        public static string UserNotInLobby => "Вы не состоите в лобби";

        public static string NotValidHashSumm => "Невалидный проверочный хэш";
        public static string TopUp => "Пополнение баланса";
        public static string WithdrawalOfFunds => "Вывод средств";

        public static string? ServersAreNotAvailable => "Нет доступных игровых серверов для начала матча";

        public static string? NotTeamForInvite => "Вы не состоите в лобби или у Вас недостаточно прав";

        public static string? CannotDefinePlayersAmount => "Нельзя поменять формат, если в лобби уже есть игроки";
        public static string SupportCover => nameof(SupportCover);
        public static string Contacts => nameof(Contacts);
        public static string Banners => nameof(Banners);
        public static string UploadedFiles => nameof(UploadedFiles);

        public static string? ServerNotHealthy => "Сервер игры не отвечает на запросы. Попробуйте другой сервер";

        public static string DemoFileNotFound => "Демо файл не был найден";
        public static string MatchEnded => nameof(MatchEnded);
        public static string MatchCanceled => nameof(MatchCanceled);
        public static string MatchPlaying => nameof(MatchPlaying);

        public static string? CannotParticipateInLobby => "Вы не можете участвовать в событиях с лобби";
        public static string AppIdentity => "app_identity";
        public static string Id => nameof(Id);
        public static string Admin => nameof(Admin);
        public static string SteamId => nameof(SteamId);
        public static string AuthentificationType => "JWT";
        public static string FindByUsers => nameof(FindByUsers);
        public static string FindById => nameof(FindById);

        public static string ReportLimitExceeded => "Превышено количество допустимых жалоб";
    }
}
