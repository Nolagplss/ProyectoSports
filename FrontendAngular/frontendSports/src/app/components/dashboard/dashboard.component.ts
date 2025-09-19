import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ReservationResponse } from '../../models/ReservationResponse';
import { ReservationFilter } from '../../models/ReservationFilter';
import { AuthService } from '../../services/auth.service';
import { ReservationService } from '../../services/reservation.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent implements OnInit{

    showFilters: boolean = false;
  reservations: ReservationResponse[] = [];
  cantidadReservas: number = 0;
  loading: boolean = false;
  error: string | null = null;

  // Filter form
  filterForm = new FormGroup({
    facilityType: new FormControl(''),
    facilityName: new FormControl(''),
    startDate: new FormControl(''),
    endDate: new FormControl('')
  });

  constructor(
    private reservationService: ReservationService,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadReservations();
  }

  loadReservations(): void {
    this.loading = true;
    this.error = null;
    
    this.reservationService.getAllReservations().subscribe({
      next: (reservations) => {
        console.log('Reservations loaded:', reservations);
        this.reservations = reservations;
        this.cantidadReservas = reservations.length;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading reservations:', error);
        this.error = 'Error loading reservations';
        this.loading = false;
        
        // If the error is authentication-related, redirect to login
        if (error.status === 401) {
          this.authService.logout();
          this.router.navigate(['/login']);
        }
      }
    });
  }

  newReservation(): void {
    this.router.navigate(['/new-reservation']);
  }

  toogleFilters(): void {
    this.showFilters = !this.showFilters;
  }

  filterReservations(): void {
    const filterData = this.filterForm.value;
    
    // Build filter object
    const filter: ReservationFilter = {};
    
    if (filterData.facilityType && filterData.facilityType !== '' && filterData.facilityType !== 'Todos') {
      filter.facilityType = filterData.facilityType;
    }
    if (filterData.facilityName) {
      filter.facilityName = filterData.facilityName;
    }
    if (filterData.startDate) {
      filter.startDate = filterData.startDate;
    }
    if (filterData.endDate) {
      filter.endDate = filterData.endDate;
    }

    this.loading = true;
    this.reservationService.filterReservations(filter).subscribe({
      next: (reservations : ReservationResponse[]) => {
        this.reservations = reservations;
        this.cantidadReservas = reservations.length;
        this.loading = false;
      },
      error: (error: Error) => {
        console.error('Error filtering reservations:', error);
        this.error = 'Error filtering reservations';
        this.loading = false;
      }
    });
  }

  clearFilters(): void {
    this.filterForm.reset();
    this.loadReservations();
  }

  cancelReservation(id: number): void {
    if (confirm('Are you sure you want to cancel this reservation?')) {
      this.reservationService.deleteReservation(id).subscribe({
         next: () => {
          this.loadReservations(); // Reload the list
         },
        error: (error) => {
          console.error('Error canceling reservation:', error);
          alert('Error canceling reservation');
         }
       });
    }
  }

  markAsNoShow(id: number): void {
    if (confirm('Mark this reservation as "No Show"?')) {
      this.reservationService.markAsNoShow(id).subscribe({
        next: () => {
          this.loadReservations(); // Reload the list
        },
        error: (error) => {
          console.error('Error marking as no show:', error);
          alert('Error marking as No Show');
        }
      });
    }
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  // Helper methods for the template
  getStatusClass(reservation: ReservationResponse): string {
    if (reservation.noShow) {
      return 'status-no-show';
    }
    return reservation.paymentCompleted ? 'status-confirmed' : 'status-pending';
  }

  getStatusText(reservation: ReservationResponse): string {
    if (reservation.noShow) {
      return 'No Show';
    }
    return reservation.paymentCompleted ? 'Confirmed' : 'Pending Payment';
  }

  getPaymentStatusText(reservation: ReservationResponse): string {
    return reservation.paymentCompleted ? 'Paid' : 'Pending';
  }

  getPaymentStatusClass(reservation: ReservationResponse): string {
    return reservation.paymentCompleted ? 'text-success' : 'text-warning';
  }

  // Format date and time for display
  formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US');
  }

  formatTimeRange(startTime: string, endTime: string): string {
    return `${startTime.substring(0, 5)} - ${endTime.substring(0, 5)}`;
  }
}
