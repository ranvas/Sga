Для запуска нового пустого BlazorServerSide приложения выполнить следующие действия:
1. Создать пустое консольное приложение.
2. Поменять в proj файле тип запускаемого проекта на <Project Sdk="Microsoft.NET.Sdk.Web">
3. Добавить в решение ссылку на Integrators.Web
4. Добавить в запускаемый проект launchSettings.json
Пример содержимого: 
-------------------------------------
{
  "iisSettings": {
    "windowsAuthentication": false,
    "anonymousAuthentication": true,
    "iisExpress": {
      "applicationUrl": "http://localhost:25401",
      "sslPort": 44324
    }
  },
  "profiles": {
    "IIS Express": {
      "commandName": "IISExpress",
      "launchBrowser": true,
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
-------------------------------------
5. Создать в запускаемом приложении файл класс наследника BlazorServerSideStartup, например TestStartUp
6. Выполнить в запускаемом приложении
var integrator = new WebIntegrator();
await integrator.RunWebAsync<TestStartUp>(args);
7. Создать в запускаемом приложении папку Pages, добавить в нее файл _Host.cshtml с содержимым @page "/".
Добавить <h1>Hello world!</h1>

Для добавления контента:

