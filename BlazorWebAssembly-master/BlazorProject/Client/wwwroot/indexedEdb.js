(function () {
    // the browser's IndexedDB APIs, along with a preconfigured database structure.

    const db = idb.openDB('EDB', 1, {
        upgrade(db) {
            db.createObjectStore('Employee', { keyPath: 'EmployeeId'});
            db.createObjectStore('TemEmployee', { keyPath: 'EmployeeId' });
            db.createObjectStore('Time', { keyPath: 'Id' });
            var departmentStore = db.createObjectStore('Department', { keyPath: "DepartmentId" });
             
            const DepartmentData = [
                { DepartmentId: 1, DepartmentName: "IT" },
                { DepartmentId: 2, DepartmentName: "HR" },
                { DepartmentId: 3, DepartmentName: "Payroll" },
                { DepartmentId: 4, DepartmentName: "Admin" }
            ];

            departmentStore.transaction.oncomplete = function (event) {
                var departmentObjectStore = db.transaction("Department", "readwrite").objectStore("Department");
                DepartmentData.forEach(function (Department) {
                    departmentObjectStore.add(Department);
                });
            };
        },
    });

    window.indexedEdb = {
        get: async (storeName, key) => (await db).transaction(storeName).store.get(key),
        getAll: async (storeName) => (await db).transaction(storeName).store.getAll(),
        getFirstFromIndex: async (storeName, indexName, direction) => {
            const cursor = await (await db).transaction(storeName).store.index(indexName).openCursor(null, direction);
            return (cursor && cursor.value) || null;
        },
        add: async (storeName, json) => {
            const store = (await db).transaction(storeName, 'readwrite').store;
            store.add(JSON.parse(json));
        },
        put: async (storeName, key, json) => {
            const store = (await db).transaction(storeName, 'readwrite').store;
            store.put(JSON.parse(json), key === null ? undefined : key)
        },
        putAllFromJson: async (storeName, json) => {
            const store = (await db).transaction(storeName, 'readwrite').store;
            JSON.parse(json).forEach(item => store.put(item));
        },
        delete: async (storeName, key) => (await db).transaction(storeName, 'readwrite').store.delete(key),
        autocompleteKeys: async (storeName, text, maxResults) => {
            const results = [];
            let cursor = await (await db).transaction(storeName).store.openCursor(IDBKeyRange.bound(text, text + '\uffff'));
            while (cursor && results.length < maxResults) {
                results.push(cursor.key);
                cursor = await cursor.continue();
            }
            return results;
        }
    };
})();
