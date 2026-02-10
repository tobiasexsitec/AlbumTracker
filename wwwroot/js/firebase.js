window.firebaseInterop = {
    _db: null,

    initialize: function (config) {
        try {
            if (!firebase.apps.length) {
                firebase.initializeApp(config);
            }
            this._db = firebase.database();
        } catch (e) {
            console.error("Firebase initialization failed:", e);
            throw e;
        }
    },

    _getRef: function (path) {
        return this._db.ref(path);
    },

    getData: async function (path) {
        try {
            const snapshot = await this._getRef(path).once("value");
            return snapshot.exists() ? JSON.stringify(snapshot.val()) : null;
        } catch (e) {
            console.error("Error getting data:", e);
            throw e;
        }
    },

    setData: async function (path, jsonData) {
        try {
            const data = JSON.parse(jsonData);
            await this._getRef(path).set(data);
        } catch (e) {
            console.error("Error setting data:", e);
            throw e;
        }
    },

    removeData: async function (path) {
        try {
            await this._getRef(path).remove();
        } catch (e) {
            console.error("Error removing data:", e);
            throw e;
        }
    },

    pushData: async function (path, jsonData) {
        try {
            const data = JSON.parse(jsonData);
            const ref = await this._getRef(path).push(data);
            return ref.key;
        } catch (e) {
            console.error("Error pushing data:", e);
            throw e;
        }
    }
};
