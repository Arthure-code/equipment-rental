import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { API_URL, EquipmentService } from './equipment.service';
import { Equipment, Rental } from './equipment';

describe('EquipmentService', () => {
  let service: EquipmentService;
  let http: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    service = TestBed.inject(EquipmentService);
    http = TestBed.inject(HttpTestingController);
  });

  afterEach(() => http.verify());

  it('asks the API for the fleet', () => {
    let fleet: Equipment[] = [];
    service.getAll().subscribe((result) => (fleet = result));

    const request = http.expectOne(API_URL);
    expect(request.request.method).toBe('GET');
    request.flush([{ id: 1, name: '260 P-Tier' }]);

    expect(fleet.length).toBe(1);
    expect(fleet[0].name).toBe('260 P-Tier');
  });

  it('asks for one machine by its id', () => {
    let machine: Equipment | undefined;
    service.get(3).subscribe((result) => (machine = result));

    http.expectOne(`${API_URL}/3`).flush({ id: 3, name: '744 P-Tier' });

    expect(machine?.id).toBe(3);
  });

  it('posts the number of days to rent', () => {
    let rental: Rental | undefined;
    service.rent(3, 5).subscribe((result) => (rental = result));

    const request = http.expectOne(`${API_URL}/3/rentals`);
    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual({ days: 5 });
    request.flush({ id: 9, equipmentId: 3, days: 5, total: 5500 });

    expect(rental?.total).toBe(5500);
  });

  it('deletes the current rental to cancel', () => {
    let done = false;
    service.cancel(3).subscribe(() => (done = true));

    const request = http.expectOne(`${API_URL}/3/rentals/current`);
    expect(request.request.method).toBe('DELETE');
    request.flush(null, { status: 204, statusText: 'No Content' });

    expect(done).toBe(true);
  });
});
