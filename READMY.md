| Сущность	| Архитектурный подход	| Статус
| :---: | :---: | :---: |
| Organization	| CQRS, собственные Handler, прямой вызов через DI	| Готово
| Site	| Application Service / Service Layer	| Готово
| Device	| CQRS + MediatR	| Запланировано
| Sensor	| Vertical Slice Architecture	| Запланировано
| MonitoringRule	| CQRS + Domain Events	| Запланировано
| Telemetry	 | Pipeline / Message-driven processing	| Запланировано
| Incident	| DDD + Domain Events	| Запланировано
| Notification	| Event-driven Architecture	| Запланировано
| DeviceSimulation	| Background Services	| Запланировано


fix route for site on org (/api/v1/organizations/{organizatioId:guid}/sites) (TODO)
