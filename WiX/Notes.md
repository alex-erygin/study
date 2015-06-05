#Заметки по WiX

Фичи
-----
* declarative approach
* unrestricted access to Windows Installer functionality
* source code instead of GUI-based assembly of information
* complete integration into application build processes
* possible integration with application development
* support for team development, both in-house and third-party
* free, open source


Getting started
----
Построение инсталятора, который:
* содержит один екзешник, одну dll, мануал, 
* создает ярлыки в панели управления и на рабочем столе
* добавляет пункт в "Установку и удаление програм"

1. Сгенерим 2 Guid: для продукта и для пакета инсталятора (GUID должен быть в верхнем регистре):
7D3FA6D5-1658-4368-B30F-4DEB9E75EF41 (для себя генерите свой!)
98810F7C-B081-4B55-ACBA-5B36D191CA39 (для себя генерите свой!)




