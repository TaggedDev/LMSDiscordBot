# Discord бот с интеграцией LMS (Google Classroom)
Проект сделан в качестве проектной работы в IT классе в рамках программы "Конференция "Инженеры будущего" направление "ИТ" для обучающихся 8-11 классов (индивидуально)

## Идея и применение:
В современном мире (в разгар и спустя год после эпидемии Covid-19) не обойтись без дистанционных систем обучения. Хотя на момент февраля 2021 года я нахожусь на оффлайн обучении, функционал этого бота может пригодится другим преподавателям, школьникам других стран (в том числе и Европы), ВУЗам, администраторам групповых занятий и просто индивидуальным преподавателям. Данный бот будет применим для создания и администрирования серверов для проведения дистанционных занятий, тематических серверов каких-либо ВУЗов.

## Команды:
* **connect** - Функция обновления баз, если это (по какой-то причине, просьба создать запрос Issue) не произошло автоматически
* **purge** - Очистка сообщений в текстовом чате, принимает количество сообщений, удаляет себя
* **info** - Информация о пользователе, принимает упоминание пользователя через @User#1234 или не принимает вовсе
* **courses** - Embed сообщение о всех классах на аккаунте авторизованного пользователя
* **class** - Информация о конкретном классе, принимает ID класса (можно получить из courses)
* **add_user** - Добавляет пользователю роль определенного класса (может быть изменено)
* **remove_user** - Удаляет роль класса (может быть изменено)
![GIF 27 02 2021 10-49-12](https://user-images.githubusercontent.com/45800215/109381144-8809f880-78e9-11eb-823b-31c0b6a8f14c.gif)

## В разработке:
* > **mute** - Заглушает пользователя от общения в текстовом и голосовом канале (может быть изменено)
* > **create** - Создает класс прямо из канала Discord
* > **class** - Упоминание всех пользователей в классе
