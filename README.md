# InfoSurge

**InfoSurge** е уеб приложение, проектирано да предоставя новинарско съдържание с функции, включително потребителска аутентикация, управление на роли и организация на новините по категории. Поддържа функции като регистрация на потребители, назначаване на роли и управление на съдържание.

## Роли
- **Нерегистриран потребител**:
- може да разглежда новини, да търси новини по ключови думи и да търси новини по категории

- **"Потребител"**:
- всички функции на нерегистрирания потребител, може да коментира под новини, да запазва новини в профила си, да реагира на новини, да променя данните си включително паролата си, и може да се абонира за категории от бутона "Абонирай се", от които иска да получава известия на подадения имейл

- **"Модератор"**:
- може да одобрява или отхвърля коментари на потребителите, преди същите да бъдат публикувани

- **"Редактор"**:
- може да създава/редактира/изтрива новини, може да създава/редактира/изтрива категориите

- **"Администратор"**:
- може да одобрява или отхвърля заявления за регистрация на потребители, може да преглежда/създава/редактира/изтрива потребители, както и да променя паролите и ролите им

## Автор
- **Име:** Станимир Желев
- **Имейл:** aaronn2022@gmail.com

## Конфигурация
За промяна на настройките на приложението, променете следните файлове:
- `appsettings.json`

## Инсталация

### Инсталация
1. Клонирайте репозиторито:
   git clone https://github.com/your-repository-url.git

2. Отваряне на проекта:
    Стартиране на InfoSurge.sln в VisualStudio

3. Стартиране на проекта:
    Натискане на клавишната комбинация Ctrl+F5 или избиране на бутана за старт

### Внимание
При сменянето на категории на началната страница от предоставеното меню трябва да се натисне бутонът "Търси", защото функцията не поддържа ajax.

Подобно е и при зареждането на коментарите, след натискането на бутона за втората страница на коментарите, трябва отново да се натисне бутонът "Коментари"
