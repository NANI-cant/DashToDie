# Dash to Die
+ [Видео геймплея](https://youtu.be/1bwlq56zYts)
  
![Image](gmeplayGif.gif)

# Описание технической части
Для разработки использовался игровой движок Unity версии 2022.3.0f1

Используются следующие библиотеки/фреймворки
+ `Zenject` - DI контейнер
+ `Addressables` - система менеджмента ассетов
+ `UniTask` - замена корутин (использование `async/await`)
+ `DoTween` - анимирование интерфейса и VFX

Архитектура:
Игра запускается с пустой Boot Scene

Для управления жизненным циклом игры реализован паттерн `State Machine` с состояниями: 
+ Boot State (начальная загрузка, настройка и инициализация)
+ Scene Load State (загрузка сцены)
+ Game Loop State (игровой цикл)

В контексте геймплея также была выделена Машина состояний с состояниями:
+ Start State - генерация карты, спавн игрока
+ Wave State - спавн врагов и бустеров
+ Rest State - отдых после прохождения уровня
+ Lose State - проигрыш

Метрика, геймплейная логика и визуал разделены для уменьшения связности.
Баланс настраивается через выделенные `ScriptableObject’ы`.

# Описание геймплея
Core геймплей:
+ Игрок уничтожает врагов с помощью стремительного удара и дополнительных способностей. 
+ Игрок проходит процедурно генерирующиеся уровни на которых спавнятся враги (3 вида) и поднимаемые бустеры (7 видов). 
+ С каждым уровнем карта становится больше, количество волн врагов, самих врагов и бустеров увеличивается согласно настраиваемым формулам.

Meta геймплей: 
+ За уничтожение врагов игрок получает валюту. Валюта тратится в магазине(магазин в разработке) на покупку скинов, оружия и зарядов дополнительных способностей
+ За прохождение уровней игрок получает “очки статов” по настраиваемой формуле. Эти очки игрок распределяет в главном меню, усиляя аспекты своих способностей (скорость накопления стамины, урон, сопротивление, скорость восстановления после удара). Очки можно распределить перед каждым “забегом”
