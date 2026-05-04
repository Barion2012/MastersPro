using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Identity;

public class SharedResource
{
}

public class RussianIdentityErrorDescriber : IdentityErrorDescriber
{

    public const string DefaultErrorMessage = "Необходимо указать: {0}";


//    private readonly IStringLocalizer _localizer;

    public RussianIdentityErrorDescriber() : base() //IStringLocalizerFactory localizer)
    {
//        _localizer = localizer.Create(;
    }


    public override IdentityError UserAlreadyInRole(string role) =>
        new IdentityError { Code = nameof(UserAlreadyInRole), Description = "Пользователь уже добавлен в роль {0}." }; 

    //
    // Сводка:
    //     Returns an Microsoft.AspNetCore.Identity.IdentityError indicating a concurrency
    //     failure.
    //
    // Возврат:
    //     An Microsoft.AspNetCore.Identity.IdentityError indicating a concurrency failure.
    public override IdentityError ConcurrencyFailure() => base.ConcurrencyFailure();
    //
    // Сводка:
    //     Returns the default Microsoft.AspNetCore.Identity.IdentityError.
    //
    // Возврат:
    //     The default Microsoft.AspNetCore.Identity.IdentityError.
    public override IdentityError DefaultError() =>
        new IdentityError { Code = nameof(DefaultError), Description = "Произошел неизвестный сбой." };
    //
    // Сводка:
    //     Returns an Microsoft.AspNetCore.Identity.IdentityError indicating the specified
    //     email is already associated with an account.
    //
    // Параметры:
    //   email:
    //     The email that is already associated with an account.
    //
    // Возврат:
    //     An Microsoft.AspNetCore.Identity.IdentityError indicating the specified email
    //     is already associated with an account.
    public override IdentityError DuplicateEmail(string email) =>
        new IdentityError { Code = nameof(DuplicateEmail), Description = $"Адрес электронной почты \"{email}\" уже используется." };
    //
    // Сводка:
    //     Returns an Microsoft.AspNetCore.Identity.IdentityError indicating the specified
    //     role name already exists.
    //
    // Параметры:
    //   role:
    //     The duplicate role.
    //
    // Возврат:
    //     An Microsoft.AspNetCore.Identity.IdentityError indicating the specific role role
    //     name already exists.
    public override IdentityError DuplicateRoleName(string role) =>
        new IdentityError { Code = nameof(DuplicateRoleName), Description = "Роль \"{0}\" уже используется" };
    //
    // Сводка:
    //     Returns an Microsoft.AspNetCore.Identity.IdentityError indicating the specified
    //     userName already exists.
    //
    // Параметры:
    //   userName:
    //     The user name that already exists.
    //
    // Возврат:
    //     An Microsoft.AspNetCore.Identity.IdentityError indicating the specified userName
    //     already exists.
    public override IdentityError DuplicateUserName(string userName) =>
        new IdentityError { Code=nameof(DuplicateUserName), Description = $"Имя \"{userName}\" уже используется."};
    //
    // Сводка:
    //     Returns an Microsoft.AspNetCore.Identity.IdentityError indicating the specified
    //     email is invalid.
    //
    // Параметры:
    //   email:
    //     The email that is invalid.
    //
    // Возврат:
    //     An Microsoft.AspNetCore.Identity.IdentityError indicating the specified email
    //     is invalid.
    public override IdentityError InvalidEmail(string email) =>
        new IdentityError { Code = nameof(InvalidEmail), Description = "Адрес электронной почты \"{0}\" является недопустимым." };
    //
    // Сводка:
    //     Returns an Microsoft.AspNetCore.Identity.IdentityError indicating the specified
    //     role name is invalid.
    //
    // Параметры:
    //   role:
    //     The invalid role.
    //
    // Возврат:
    //     An Microsoft.AspNetCore.Identity.IdentityError indicating the specific role role
    //     name is invalid.
    public override IdentityError InvalidRoleName(string role) =>
        new IdentityError { Code = nameof(InvalidRoleName), Description = "Роль {0} не существует." };
    //
    // Сводка:
    //     Returns an Microsoft.AspNetCore.Identity.IdentityError indicating an invalid
    //     token.
    //
    // Возврат:
    //     An Microsoft.AspNetCore.Identity.IdentityError indicating an invalid token.
    public override IdentityError InvalidToken()=>
    new IdentityError { Code = nameof(InvalidToken), Description = "Недопустимый маркер." };
//
// Сводка:
//     Returns an Microsoft.AspNetCore.Identity.IdentityError indicating the specified
//     user userName is invalid.
//
// Параметры:
//   userName:
//     The user name that is invalid.
//
// Возврат:
//     An Microsoft.AspNetCore.Identity.IdentityError indicating the specified user
//     userName is invalid.
public override IdentityError InvalidUserName(string userName)=>
                new IdentityError { Code = nameof(InvalidUserName), Description = "Недопустимое имя пользователя: {0}. Имена пользователей могут содержать только буквы и цифры." };

    //
    // Сводка:
    //     Returns an Microsoft.AspNetCore.Identity.IdentityError indicating an external
    //     login is already associated with an account.
    //
    // Возврат:
    //     An Microsoft.AspNetCore.Identity.IdentityError indicating an external login is
    //     already associated with an account.
    public override IdentityError LoginAlreadyAssociated()=>
                new IdentityError { Code = nameof(LoginAlreadyAssociated), Description = "Пользователь с таким внешним именем уже существует." };

    //
    // Сводка:
    //     Returns an Microsoft.AspNetCore.Identity.IdentityError indicating a password
    //     mismatch.
    //
    // Возврат:
    //     An Microsoft.AspNetCore.Identity.IdentityError indicating a password mismatch.
    public override IdentityError PasswordMismatch()=>
                new IdentityError { Code = nameof(PasswordMismatch), Description = "Неправильный пароль." };

    //
    // Сводка:
    //     Returns an Microsoft.AspNetCore.Identity.IdentityError indicating a password
    //     entered does not contain a numeric character, which is required by the password
    //     policy.
    //
    // Возврат:
    //     An Microsoft.AspNetCore.Identity.IdentityError indicating a password entered
    //     does not contain a numeric character.
    public override IdentityError PasswordRequiresDigit()=>
                new IdentityError { Code = nameof(PasswordRequiresDigit), Description = "В пароле должна быть по крайней мере одна цифра (0–9)." };

    //
    // Сводка:
    //     Returns an Microsoft.AspNetCore.Identity.IdentityError indicating a password
    //     entered does not contain a lower case letter, which is required by the password
    //     policy.
    //
    // Возврат:
    //     An Microsoft.AspNetCore.Identity.IdentityError indicating a password entered
    //     does not contain a lower case letter.
    public override IdentityError PasswordRequiresLower()=>
                new IdentityError { Code = nameof(PasswordRequiresLower), Description = "В пароле должен быть по крайней мере один символ в нижнем регистре (\"a\"–\"z\")." };

    //
    // Сводка:
    //     Returns an Microsoft.AspNetCore.Identity.IdentityError indicating a password
    //     entered does not contain a non-alphanumeric character, which is required by the
    //     password policy.
    //
    // Возврат:
    //     An Microsoft.AspNetCore.Identity.IdentityError indicating a password entered
    //     does not contain a non-alphanumeric character.
    public override IdentityError PasswordRequiresNonAlphanumeric()=>
                new IdentityError { Code = nameof(PasswordRequiresNonAlphanumeric), Description = "В пароле должен быть по крайней мере один небуквенный или нецифровой символ." };

    //
    // Сводка:
    //     Returns an Microsoft.AspNetCore.Identity.IdentityError indicating a password
    //     does not meet the minimum number uniqueChars of unique chars.
    //
    // Параметры:
    //   uniqueChars:
    //     The number of different chars that must be used.
    //
    // Возврат:
    //     An Microsoft.AspNetCore.Identity.IdentityError indicating a password does not
    //     meet the minimum number uniqueChars of unique chars.
    public override IdentityError PasswordRequiresUniqueChars(int uniqueChars)=>
                new IdentityError { Code = nameof(PasswordRequiresUniqueChars), Description = "Пароль должен содержать по крайней мере {0} различных символов." };

    //
    // Сводка:
    //     Returns an Microsoft.AspNetCore.Identity.IdentityError indicating a password
    //     entered does not contain an upper case letter, which is required by the password
    //     policy.
    //
    // Возврат:
    //     An Microsoft.AspNetCore.Identity.IdentityError indicating a password entered
    //     does not contain an upper case letter.
    public override IdentityError PasswordRequiresUpper()=>
                new IdentityError { Code = nameof(PasswordRequiresUpper), Description = "В пароле должен быть по крайней мере один символ в верхнем регистре (\"A\"–\"Z\")." };

    //
    // Сводка:
    //     Returns an Microsoft.AspNetCore.Identity.IdentityError indicating a password
    //     of the specified length does not meet the minimum length requirements.
    //
    // Параметры:
    //   length:
    //     The length that is not long enough.
    //
    // Возврат:
    //     An Microsoft.AspNetCore.Identity.IdentityError indicating a password of the specified
    //     length does not meet the minimum length requirements.
    public override IdentityError PasswordTooShort(int length)=>
                new IdentityError { Code = nameof(PasswordTooShort), Description = "Пароли должны содержать не меньше {0} символов." };

    //
    // Сводка:
    //     Returns an Microsoft.AspNetCore.Identity.IdentityError indicating a recovery
    //     code was not redeemed.
    //
    // Возврат:
    //     An Microsoft.AspNetCore.Identity.IdentityError indicating a recovery code was
    //     not redeemed.
    public override IdentityError RecoveryCodeRedemptionFailed()=>
                new IdentityError { Code = nameof(RecoveryCodeRedemptionFailed), Description = "Код восстановления не был активирован." };

    //
    // Сводка:
    //     Returns an Microsoft.AspNetCore.Identity.IdentityError indicating a user already
    //     has a password.
    //
    // Возврат:
    //     An Microsoft.AspNetCore.Identity.IdentityError indicating a user already has
    //     a password.
    public override IdentityError UserAlreadyHasPassword()=>
                new IdentityError { Code = nameof(UserAlreadyHasPassword), Description = "Пароль пользователя уже задан." };

    //
    // Сводка:
    //     Returns an Microsoft.AspNetCore.Identity.IdentityError indicating a user is already
    //     in the specified role.
    //
    // Параметры:
    //   role:
    //     The duplicate role.
    //
    // Возврат:
    //     An Microsoft.AspNetCore.Identity.IdentityError indicating a user is already in
    //     the specified role.
    public override IdentityError UserLockoutNotEnabled() =>
        new IdentityError { Code = nameof(UserLockoutNotEnabled), Description = "Блокировка для этого пользователя не включена." };
    //
    // Сводка:
    //     Returns an Microsoft.AspNetCore.Identity.IdentityError indicating a user is not
    //     in the specified role.
    //
    // Параметры:
    //   role:
    //     The duplicate role.
    //
    // Возврат:
    //     An Microsoft.AspNetCore.Identity.IdentityError indicating a user is not in the
    //     specified role.
    public override IdentityError UserNotInRole(string role)=>
        new IdentityError { Code = nameof(UserNotInRole), Description = "Пользователь не содержится в роли {0}." };

    /*  
    
   
    StoreNotIUserClaimStore=Магазин не реализует IUserClaimStore<TUser>.
    NoTwoFactorProvider=IUserTwoFactorProvider для "{0}" не зарегистрирован.
    StoreNotIUserEmailStore=В хранилище не реализован IUserEmailStore<TUser>.
    
    StoreNotIUserLockoutStore=Хранилище не реализует IUserLockoutStore<TUser>.
    NoTokenProvider=IUserTokenProvider не зарегистрирован.
    StoreNotIUserRoleStore=Магазин не реализует IUserRoleStore<TUser>.
    StoreNotIUserLoginStore=Магазин не реализует IUserLoginStore<TUser>.
    
    StoreNotIUserPhoneNumberStore=В хранилище не реализован IUserPhoneNumberStore<TUser>.
    StoreNotIUserConfirmationStore=В хранилище не реализован IUserConfirmationStore<TUser>.
    
    PropertyTooShort={0} не может иметь значение NULL или быть пустым.
    
    
    StoreNotIUserPasswordStore=В хранилище не реализован IUserPasswordStore<TUser>.
 
    UserIdNotFound=Не удается найти UserId.
    
    
    UserNameNotFound=Пользователь {0} не существует.
    StoreNotIQueryableRoleStore=В хранилище не реализован IQueryableRoleStore<TRole>.

    StoreNotIUserTwoFactorStore=В хранилище не реализован IUserTwoFactorStore<TUser>.
    
 
    
    StoreNotIQueryableUserStore=В хранилище не реализован IQueryableUserStore<TUser>.
    StoreNotIUserSecurityStampStore=В хранилище не реализован IUserSecurityStampStore<TUser>.
 
    */
}