import { Injectable, signal } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class HeaderService {

  
  showHeader = signal(true);
  showNavLinks = signal(true);

  hideHeader() {
    this.showHeader.set(false);
  }

  showHeaderOnlyLogo() {
    this.showHeader.set(true);
    this.showNavLinks.set(false);
  }

  showHeaderWithLinks() {
    this.showHeader.set(true);
    this.showNavLinks.set(true);
  }
}
