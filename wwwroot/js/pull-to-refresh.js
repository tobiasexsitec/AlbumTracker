// Pull-to-refresh functionality
let startY = 0;
let currentY = 0;
let pulling = false;
const threshold = 80;

document.addEventListener('touchstart', (e) => {
    if (window.scrollY === 0) {
        startY = e.touches[0].pageY;
        pulling = false;
    }
}, { passive: true });

document.addEventListener('touchmove', (e) => {
    if (window.scrollY === 0 && startY) {
        currentY = e.touches[0].pageY;
        const pullDistance = currentY - startY;
        
        if (pullDistance > 0) {
            pulling = true;
            // Visual feedback handled by browser's native behavior
        }
    }
}, { passive: true });

document.addEventListener('touchend', async () => {
    if (pulling && (currentY - startY) > threshold) {
        // Check for service worker update
        if ('serviceWorker' in navigator) {
            const registration = await navigator.serviceWorker.getRegistration();
            if (registration) {
                await registration.update();
            }
        }
        
        // Force reload to get latest version
        window.location.reload();
    }
    pulling = false;
    startY = 0;
    currentY = 0;
});
