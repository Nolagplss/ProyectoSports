import { Component, signal } from '@angular/core';
import { Router, RouterLink, RouterOutlet } from '@angular/router';
import { HeaderComponent } from './components/header/header.component';
import { HeaderService } from './services/header.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, HeaderComponent, RouterLink],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
 
  title = signal('frontendSports');

  constructor(private router: Router, public headerService: HeaderService) {

     this.router.events.subscribe(() => {
      const currentUrl = this.router.url;

      // aquí decides según la ruta qué hacer
      if (currentUrl === '/') {
        this.headerService.showHeaderWithLinks();
      } else if (currentUrl.startsWith('/login') || currentUrl.startsWith('/register')) {
        this.headerService.showHeaderWithLinks();
      } else if (currentUrl.startsWith('/dashboard')) {
        this.headerService.showHeaderOnlyLogo();
      } else {
        this.headerService.showHeaderWithLinks();
      }
    });


  }

  isAuthRoute(): boolean {
  const currentUrl = this.router.url.split('#')[0]; // ignora fragmentos
  return currentUrl != '/';
  }



}
