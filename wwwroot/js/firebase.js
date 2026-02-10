window.firebaseInterop = {
    _db: null,

    initialize: function (config) {
        if (!firebase.apps.length) {
            firebase.initializeApp(config);
        }
        this._db = firebase.database();
    },

    _getRef: function (path) {
        return this._db.ref(path);
    },

    getData: async function (path) {
        const snapshot = await this._getRef(path).once("value");
        return snapshot.exists() ? JSON.stringify(snapshot.val()) : null;
    },

    setData: async function (path, jsonData) {
        const data = JSON.parse(jsonData);
        await this._getRef(path).set(data);
    },

    removeData: async function (path) {
        await this._getRef(path).remove();
    },

    pushData: async function (path, jsonData) {
        const data = JSON.parse(jsonData);
        const ref = await this._getRef(path).push(data);
        return ref.key;
    }
};
