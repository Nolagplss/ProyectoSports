import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { Router, RouterLink, RouterOutlet } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { MenuItem } from '../../models/MenuItem';



@Component({
  selector: 'app-main-layout',
  imports: [CommonModule, RouterLink, RouterOutlet],
  standalone: true,
  templateUrl: './main-layout.component.html',
  styleUrl: './main-layout.component.css'
})
export class MainLayoutComponent implements OnInit{

  activeView: string = 'reservations';
  userRole: string | null = null;
  userName: string | null = null;
  sidebarOpen: boolean = true;

  menuItems: MenuItem[] = [
    {
      id: 'reservations',
      label: 'My Reservations',
      icon: 'calendar',
      route: '/main/reservations',
      roles: ['Socio', 'Encargado', 'Administrador']
    },
    {
      id: 'users',
      label: 'Users Management',
      icon: 'users',
      route: '/main/users',
      roles: ['Encargado', 'Administrador']
    },
    {
      id: 'facilities',
      label: 'Facilities',
      icon: 'building',
      route: '/main/facilities',
      roles: ['Encargado', 'Administrador']
    },
    {
      id: 'reports',
      label: 'Reports',
      icon: 'file-text',
      route: '/main/reports',
      roles: ['Encargado', 'Administrador']
    },
    {
      id: 'settings',
      label: 'Settings',
      icon: 'settings',
      route: '/main/settings',
      roles: ['Socio', 'Encargado', 'Administrador']
    }
  ];
 constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.userRole = this.authService.getUserRole();
    this.userName = this.authService.getUserName();
  }

  getVisibleMenuItems(): MenuItem[] {
    return this.menuItems.filter(item =>
      item.roles.includes(this.userRole || '')
    );
  }

  selectView(viewId: string): void {
    this.activeView = viewId;
    const menuItem = this.menuItems.find(item => item.id === viewId);
    if (menuItem) {
      this.router.navigate([menuItem.route]);
    }
  }

  toggleSidebar(): void {
    this.sidebarOpen = !this.sidebarOpen;
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  getIconClass(iconName: string): string {
    const iconMap: { [key: string]: string } = {
      'calendar': '📅',
      'users': '👥',
      'building': '🏢',
      'file-text': '📄',
      'settings': '⚙️'
    };
    return iconMap[iconName] || '•';
  }
}
