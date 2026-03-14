// Force check for updates when app opens
window.addEventListener('load', async () => {
    if ('serviceWorker' in navigator) {
        const registration = await navigator.serviceWorker.getRegistration();
        if (registration) {
            // Check for updates immediately
            registration.update();
            
            // Listen for new service worker
            registration.addEventListener('updatefound', () => {
                const newWorker = registration.installing;
                newWorker.addEventListener('statechange', () => {
                    if (newWorker.state === 'installed' && navigator.serviceWorker.controller) {
                        // New version available, reload to activate it
                        if (confirm('New version available! Reload to update?')) {
                            window.location.reload();
                        }
                    }
                });
            });
        }
    }
});
