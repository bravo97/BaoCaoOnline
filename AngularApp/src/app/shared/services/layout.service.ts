import { Injectable, signal } from '@angular/core';

@Injectable({
    providedIn: 'root'
})
export class LayoutService {
    // Mobile Sidebar State
    mobileSidebarOpen = signal(false);

    toggleMobileSidebar() {
        this.mobileSidebarOpen.update(v => !v);
    }

    closeMobileSidebar() {
        this.mobileSidebarOpen.set(false);
    }
}
