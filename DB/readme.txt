Справочники
- сотрудники - Employees (
			ФИО - fio, 
			*подразделение - Divisions_id, 
			*должность - Posts_id, 
			дата приёма - dateHire, 
			дата уволнени - dateDismissal)

- должности - Posts (
			name - наименование)

- изделия - Products (
			название - name, 
			заводской_номер - serialNo, 
			*единица_измерения - Units_id
			*место_хранения - Locations_id)

- единицы_измерения - Units (
			название - name)

- места_хранения - Locations (
			название - name, 
			*кладовщик - Employees_id)

- подразделения - Divisions (
			название - name)

- материалы\коплектуюшие - Materials (
			название - name, 
			*единица_измерения - Units_id, 
			*место_хранения - Locations_id, 
			количество - count, 
			цена - price)

Таблица связи "Изделие_Материалы" (Product_Materials):
			id (Primary Key)
			product_id (Foreign Key из таблицы "Изделия")
			material_id (Foreign Key из таблицы "Материалы")
			quantity (Количество материала для конкретного изделия, если нужно)

- контрагенты - Counterparties (
			name - ФИО/наименование
			legalAddress - юридический адрес
			phoneNomber - номер телефона
			postalAddress - почтовый адресс
			ynp - УНП
			type [fiz/yr] - тип [физ/юр]
			okulp - ОКЮЛП
			okpo - ОКПО
			oked - ОКЕД)

- Остатки (название, количество)

Документы
- договора - Contracts (
			*контрагент - Counterparties_id, 
			дата заключения - date, 
			сумма - summ, 
			*изделия - Products_id, 
			количество - count)

- поступления - Receipts (
			*материал\комплектуюшие - Materials_id
			сумма - summ, 
			дата - date
			количество - count
			*контрагент - Counterparties_id)

- реализация - Sales (
			*изделие - Products_id
			*договор - Contracts_id
			сумма - summ, 
			дата реализация - date)

Отчёты
- по: изделие, дате, подразделение, сотрудники