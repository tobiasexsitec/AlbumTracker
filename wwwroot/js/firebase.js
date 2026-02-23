window.firebaseInterop = {
    _db: null,

    initialize: function (configJson) {
        try {
            const config = JSON.parse(configJson);
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

    setData: async function (path, json) {
        try {
            await this._getRef(path).set(JSON.parse(json));
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

    pushData: async function (path, json) {
        try {
            const ref = await this._getRef(path).push(JSON.parse(json));
            return ref.key;
        } catch (e) {
            console.error("Error pushing data:", e);
            throw e;
        }
    },

    // --- Authentication ---

    signInWithGoogle: function () {
        const provider = new firebase.auth.GoogleAuthProvider();
        firebase.auth().signInWithRedirect(provider);
    },

    checkRedirectResult: async function () {
        try {
            await firebase.auth().getRedirectResult();
        } catch (e) {
            console.error("Redirect sign-in failed:", e);
        }
    },

    signOut: async function () {
        await firebase.auth().signOut();
    },

    getCurrentUser: function () {
        const user = firebase.auth().currentUser;
        if (!user) return null;
        return JSON.stringify({
            uid: user.uid,
            displayName: user.displayName,
            email: user.email,
            photoUrl: user.photoURL
        });
    },

    onAuthStateChanged: function (dotNetRef) {
        firebase.auth().onAuthStateChanged(function (user) {
            if (user) {
                dotNetRef.invokeMethodAsync('OnUserSignedIn', JSON.stringify({
                    uid: user.uid,
                    displayName: user.displayName,
                    email: user.email,
                    photoUrl: user.photoURL
                }));
            } else {
                dotNetRef.invokeMethodAsync('OnUserSignedOut');
            }
        });
    }
};
