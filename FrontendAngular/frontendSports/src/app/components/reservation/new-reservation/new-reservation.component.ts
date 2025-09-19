import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { FacilityService} from '../../../services/facility.service';
import { ReservationService } from '../../../services/reservation.service';
import { AuthService } from '../../../services/auth.service';
import { FacilityDTO } from '../../../models/FacilityDTO';
import { CreateReservationRequest } from '../../../models/CreateReservationRequest';
import { Sport } from '../../../models/Sport';
import { FormsModule } from '@angular/forms';
import { TimeSlot } from '../../../models/TimeSlot';

export interface DateOption {
  dateString: string;
  displayDate: string;
  dayName: string;
  dayNumber: number;
  monthName: string;
  isToday: boolean;
}



@Component({
  selector: 'app-new-reservation',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './new-reservation.component.html',
  styleUrls: ['./new-reservation.component.css']
})
export class NewReservationComponent implements OnInit {

  currentStep = 1;
  loading = false;
  isSubmitting = false;
  termsAccepted = false;

  //Selecciones del usuario
  selectedFacility: FacilityDTO | null = null;
  selectedDate: DateOption | null = null;
  selectedTimeSlot: TimeSlot | null = null;
  selectedDuration = 1;

  //Datos disponibles
 availableSports: Sport[] = [
  { type: 'Soccer', name: 'Soccer', icon: '⚽', description: 'Soccer field', maxDuration: 2, availableFacilities: 3 },
  { type: 'Tennis', name: 'Tennis', icon: '🎾', description: 'Tennis court', maxDuration: 1, availableFacilities: 2 },
  // …
];

  selectedSport: Sport | null = null;

  availableFacilities: FacilityDTO[] = [];
  availableDates: DateOption[] = [];
  availableTimeSlots: TimeSlot[] = [];
  availableDurations: number[] = [1, 2, 3];

  constructor(
    private router: Router,
    private facilityService: FacilityService,
    private reservationService: ReservationService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.generateAvailableDates();
    this.loadAvailableSports();
  }
  loadAvailableSports() {
    this.loading = true;
    this.facilityService.getAllFacilities().subscribe({
      next: (facilities) => {
        //Agrupar instalaciones por tipo
        const grouped: { [type: string]: FacilityDTO[] } = {};
        facilities.forEach(f => {
          if (!grouped[f.type]) grouped[f.type] = [];
          grouped[f.type].push(f);
        });

        //Convertir cada grupo a un "Sport"
        this.availableSports = Object.keys(grouped).map(type => ({
          type,
          name: type,
          icon: this.getSportIcon(type),
          description: `${type} facilities`,
          maxDuration: Math.max(...grouped[type].map(f => f.maxReservationHours)),
          availableFacilities: grouped[type].length
        }));

        this.loading = false;
      },
      error: (err) => {
        console.error("Error loading sports:", err);
        this.loading = false;
      }
    });
  }

   private getSportIcon(type: string): string {
    const icons: Record<string, string> = {
      Soccer: '⚽',
      Tennis: '🎾',
      Padel: '🥎',
      Basketball: '🏀',
      Pool: '🏊',
      Gym: '💪'
    };
    return icons[type] || '❓';
  }
  //Navegación
  goBack() { this.router.navigate(['/dashboard']); }
  nextStep() { 
    if (this.currentStep < 4) {
      this.currentStep++;
      if (this.currentStep === 2) this.loadFacilities();
      if (this.currentStep === 3) this.loadAvailableTimeSlots();
    }
  }
  previousStep() { if (this.currentStep > 1) this.currentStep--; }

  //Paso 1: seleccionar deporte
  selectSport(sport: Sport) {
    this.selectedSport = sport;
    this.selectedFacility = null;
    this.selectedDate = null;
    this.selectedTimeSlot = null;
  }

  //Paso 2: seleccionar instalación
  loadFacilities() {
    if (!this.selectedSport) return;
    this.loading = true;
    this.facilityService.getAllFacilities().subscribe({
      next: (facilities) => {
        //Filtrar por tipo de deporte
       this.availableFacilities = facilities.filter(f => f.type === this.selectedSport!.type);
        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading facilities:', err);
        this.loading = false;
      }
    });
  }

  selectFacility(facility: FacilityDTO) {
    this.selectedFacility = facility;
    this.selectedDate = null;
    this.selectedTimeSlot = null;
  }

  //Paso 3: fechas y horarios
  generateAvailableDates() {
    const dates: DateOption[] = [];
    const today = new Date();
    for (let i = 0; i < 15; i++) {
      const date = new Date();
      date.setDate(today.getDate() + i);
      dates.push({
        dateString: date.toISOString().split('T')[0],
        displayDate: date.toLocaleDateString('en-US', { weekday: 'long', day: 'numeric', month: 'long' }),
        dayName: date.toLocaleDateString('en-US', { weekday: 'short' }),
        dayNumber: date.getDate(),
        monthName: date.toLocaleDateString('en-US', { month: 'short' }),
        isToday: i === 0
      });
    }
    this.availableDates = dates;
  }

  selectDate(date: DateOption) {
    this.selectedDate = date;
    this.selectedTimeSlot = null;
    this.loadAvailableTimeSlots();
  }

  loadAvailableTimeSlots() {
  if (!this.selectedFacility || !this.selectedDate) return;
  this.loading = true;

  this.reservationService.getAvailableSlots(this.selectedFacility.facilityId, this.selectedDate.dateString)
    .subscribe({
      next: (slots) => {
        this.availableTimeSlots = slots.map(s => ({
          startTime: s.startTime,
          endTime: s.endTime,
          duration: 1,
          isOccupied: false,
          isDisabled: false
        }));
        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading available slots:', err);
        this.availableTimeSlots = [];
        this.loading = false;
      }
    });
}



  selectTimeSlot(slot: TimeSlot) {
    this.selectedTimeSlot = slot;
    this.selectedDuration = slot.duration;
  }

  selectDuration(duration: number) { this.selectedDuration = duration; }

  getEndTime(): string {
    if (!this.selectedTimeSlot) return '';
    const startHour = parseInt(this.selectedTimeSlot.startTime.split(':')[0]);
    const endHour = startHour + this.selectedDuration;
    return `${endHour.toString().padStart(2,'0')}:00`;
  }

  calculatePrice(): number {
    if (!this.selectedFacility || !this.selectedDuration) return 0;
    const priceMap: {[key: string]: number} = {
      'Soccer': 25, 'Tennis': 20, 'Padel': 18, 'Basketball': 15, 'Pool': 12, 'Gym': 10
    };
    return (priceMap[this.selectedFacility.type] || 20) * this.selectedDuration;
  }

  //Paso 4: confirmar
 confirmReservation() {
  if (!this.selectedFacility || !this.selectedDate || !this.selectedTimeSlot || !this.termsAccepted) return;

  this.isSubmitting = true;

  const reservationData: CreateReservationRequest = {
    facilityId: this.selectedFacility.facilityId,
    reservationDate: this.selectedDate.dateString,
    startTime: this.selectedTimeSlot.startTime,
    endTime: this.getEndTime()
  };

  this.reservationService.createReservation(reservationData).subscribe({
    next: () => {
      this.isSubmitting = false;
      alert('Reservation created successfully!');
      this.router.navigate(['/dashboard']);
    },
    error: (err) => {
      this.isSubmitting = false;
      console.error('Error creating reservation:', err);
      alert('Error creating reservation. Please try again.');
      if (err.status === 401) {
        this.authService.logout();
        this.router.navigate(['/login']);
      }
    }
  });
}

canSelectDuration(duration: number): boolean {
  return this.selectedFacility ? duration <= this.selectedFacility.maxReservationHours : false;
}


}
