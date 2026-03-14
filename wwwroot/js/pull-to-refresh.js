// Pull-to-refresh functionality
let startY = 0;
let currentY = 0;
let pulling = false;
const threshold = 80;
let pullIndicator = null;

// Create pull indicator element
function createPullIndicator() {
    const indicator = document.createElement('div');
    indicator.className = 'pull-to-refresh-indicator';
    indicator.innerHTML = `
        <div class="pull-spinner">
            <svg viewBox="0 0 50 50">
                <circle cx="25" cy="25" r="20" fill="none" stroke-width="4"></circle>
            </svg>
        </div>
    `;
    document.body.appendChild(indicator);
    return indicator;
}

// Initialize indicator
document.addEventListener('DOMContentLoaded', () => {
    pullIndicator = createPullIndicator();
});

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

            // Show and update indicator
            if (pullIndicator) {
                const progress = Math.min(pullDistance / threshold, 1);
                pullIndicator.style.opacity = progress;
                pullIndicator.style.transform = `translateY(${Math.min(pullDistance * 0.5, 60)}px)`;

                // Add spinning class when threshold reached
                if (progress >= 1) {
                    pullIndicator.classList.add('ready');
                } else {
                    pullIndicator.classList.remove('ready');
                }
            }
        }
    }
}, { passive: true });

document.addEventListener('touchend', async () => {
    if (pulling && (currentY - startY) > threshold) {
        // Show loading state
        if (pullIndicator) {
            pullIndicator.classList.add('loading');
        }

        // Check for service worker update
        if ('serviceWorker' in navigator) {
            const registration = await navigator.serviceWorker.getRegistration();
            if (registration) {
                await registration.update();
            }
        }

        // Force reload to get latest version
        window.location.reload();
    } else {
        // Reset indicator
        if (pullIndicator) {
            pullIndicator.style.opacity = '0';
            pullIndicator.style.transform = 'translateY(-20px)';
            pullIndicator.classList.remove('ready');
        }
    }

    pulling = false;
    startY = 0;
    currentY = 0;
});
