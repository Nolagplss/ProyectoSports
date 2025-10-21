import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './components/auth/login/login.component';
import { RegisterComponent } from './components/auth/register/register.component';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { NewReservationComponent } from './components/reservation/new-reservation/new-reservation.component';
import { MainLayoutComponent } from './main/main-layout/main-layout.component';
import { AuthGuard } from './services/auth-guard.service';
import { AppComponent } from './app.component';

export const routes: Routes = [
  { path: '', component: AppComponent },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  
  {
    path: 'app',
    component: MainLayoutComponent,
    canActivate: [AuthGuard],
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      { path: 'dashboard', component: DashboardComponent },
      { path: 'new-reservation', component: NewReservationComponent },
      //{ path: 'users', component: UsersComponent }
    ]
  },

  // Rutas legacies (opcional, redireccionar a la nueva estructura)
  { path: 'dashboard', redirectTo: 'app/dashboard' },
  { path: 'new-reservation', redirectTo: 'app/new-reservation' }

];
