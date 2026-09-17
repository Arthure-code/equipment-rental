import { HttpErrorResponse } from '@angular/common/http';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { Observable, of, throwError } from 'rxjs';
import { EquipmentList } from './equipment-list';
import { EquipmentService } from './equipment.service';
import { Equipment, Rental } from './equipment';

const machine = (id: number, category: string, available = true): Equipment => ({
  id,
  name: `Machine ${id}`,
  category,
  description: '',
  dailyRate: 100 * id,
  imageUrl: `https://images.unsplash.com/photo-${id}`,
  available,
  rentedFrom: available ? null : '2026-09-17T18:00:00Z',
  rentedUntil: available ? null : '2026-09-20T18:00:00Z',
});

// The service as the list sees it: a fleet that changes after a rental.
class ServiceStub {
  fleet: Equipment[] = [
    machine(1, 'Earthmoving'),
    machine(2, 'Earthmoving', false),
    machine(3, 'Planting'),
  ];
  rentAnswer: Observable<Rental> = of({
    id: 1,
    equipmentId: 1,
    startsOn: '',
    endsOn: '',
    days: 3,
    total: 300,
  });
  calls: string[] = [];

  getAll(): Observable<Equipment[]> {
    this.calls.push('getAll');
    return of(this.fleet);
  }

  rent(id: number, days: number): Observable<Rental> {
    this.calls.push(`rent ${id} ${days}`);
    this.fleet = this.fleet.map((e) => (e.id === id ? { ...e, available: false } : e));
    return this.rentAnswer;
  }

  cancel(id: number): Observable<void> {
    this.calls.push(`cancel ${id}`);
    this.fleet = this.fleet.map((e) => (e.id === id ? { ...e, available: true } : e));
    return of(undefined);
  }
}

describe('EquipmentList', () => {
  let fixture: ComponentFixture<EquipmentList>;
  let stub: ServiceStub;

  const root = () => fixture.nativeElement as HTMLElement;
  const text = (testId: string) =>
    root().querySelector(`[data-testid="${testId}"]`)?.textContent?.replace(/\s+/g, ' ').trim();
  const cards = () => root().querySelectorAll('app-equipment-card').length;

  beforeEach(async () => {
    stub = new ServiceStub();
    await TestBed.configureTestingModule({
      imports: [EquipmentList],
      providers: [provideRouter([]), { provide: EquipmentService, useValue: stub }],
    }).compileComponents();
    fixture = TestBed.createComponent(EquipmentList);
    fixture.autoDetectChanges();
    await fixture.whenStable();
  });

  it('shows every machine and counts the available ones', () => {
    expect(cards()).toBe(3);
    expect(text('summary')).toBe('3 machines, 2 available today');
  });

  it('filters by category with one button per category found', async () => {
    const buttons = root().querySelectorAll<HTMLButtonElement>('[data-testid="categories"] button');
    expect([...buttons].map((b) => b.textContent?.trim())).toEqual([
      'All',
      'Earthmoving',
      'Planting',
    ]);

    buttons[2].click();
    await fixture.whenStable();

    expect(cards()).toBe(1);
    expect(text('summary')).toBe('1 machines, 1 available today');
    expect(buttons[2].getAttribute('aria-pressed')).toBe('true');
  });

  it('rents through the service, says so with the price, and reloads', async () => {
    root()
      .querySelector<HTMLButtonElement>('[data-testid="card-1"] [data-testid="durations"] button')
      ?.click();
    await fixture.whenStable();

    expect(stub.calls).toEqual(['getAll', 'rent 1 1', 'getAll']);
    expect(text('message')).toBe('Machine 1 rented for 1 day, $300.');
    expect(text('summary')).toBe('3 machines, 1 available today');
  });

  it('cancels through the service and reloads', async () => {
    root()
      .querySelector<HTMLButtonElement>('[data-testid="card-2"] button.btn-outline-secondary')
      ?.click();
    await fixture.whenStable();

    expect(stub.calls).toEqual(['getAll', 'cancel 2', 'getAll']);
    expect(text('message')).toBe('Rental of Machine 2 cancelled.');
    expect(text('summary')).toBe('3 machines, 3 available today');
  });

  it('explains a 409 as the machine being already rented', async () => {
    stub.rentAnswer = throwError(() => new HttpErrorResponse({ status: 409 }));

    root()
      .querySelector<HTMLButtonElement>('[data-testid="card-1"] [data-testid="durations"] button')
      ?.click();
    await fixture.whenStable();

    expect(text('message')).toBe('Machine 1 is already rented.');
    expect(
      root().querySelector('[data-testid="message"]')?.classList.contains('alert-danger'),
    ).toBe(true);
  });
});
