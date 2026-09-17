import { HttpErrorResponse } from '@angular/common/http';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { Observable, of, throwError } from 'rxjs';
import { EquipmentDetail } from './equipment-detail';
import { EquipmentService } from './equipment.service';
import { Equipment, Rental } from './equipment';

const planter: Equipment = {
  id: 5,
  name: '1725',
  category: 'Planting',
  description: 'Planter.',
  dailyRate: 480,
  imageUrl: 'https://images.unsplash.com/photo-1655048424318-2d56ccf9a51a',
  available: true,
  rentedFrom: null,
  rentedUntil: null,
};

class ServiceStub {
  answer: Observable<Equipment> = of(planter);
  calls: string[] = [];

  get(id: number): Observable<Equipment> {
    this.calls.push(`get ${id}`);
    return this.answer;
  }

  rent(id: number, days: number): Observable<Rental> {
    this.calls.push(`rent ${id} ${days}`);
    this.answer = of({ ...planter, available: false, rentedUntil: '2026-09-22T18:00:00Z' });
    return of({ id: 1, equipmentId: id, startsOn: '', endsOn: '', days, total: 480 * days });
  }

  cancel(id: number): Observable<void> {
    this.calls.push(`cancel ${id}`);
    this.answer = of(planter);
    return of(undefined);
  }
}

describe('EquipmentDetail', () => {
  let fixture: ComponentFixture<EquipmentDetail>;
  let stub: ServiceStub;

  const root = () => fixture.nativeElement as HTMLElement;
  const text = (testId: string) =>
    root().querySelector(`[data-testid="${testId}"]`)?.textContent?.replace(/\s+/g, ' ').trim();

  const open = async (id: string) => {
    fixture = TestBed.createComponent(EquipmentDetail);
    fixture.componentRef.setInput('id', id);
    fixture.autoDetectChanges();
    await fixture.whenStable();
  };

  beforeEach(async () => {
    stub = new ServiceStub();
    await TestBed.configureTestingModule({
      imports: [EquipmentDetail],
      providers: [provideRouter([]), { provide: EquipmentService, useValue: stub }],
    }).compileComponents();
  });

  it('loads the machine named in the address and shows its card', async () => {
    await open('5');

    expect(stub.calls).toEqual(['get 5']);
    expect(root().querySelector('h2')?.textContent?.trim()).toBe('1725');
    expect(text('status')).toBe('Available');
  });

  it('rents from the card and shows the machine as rented', async () => {
    await open('5');

    root().querySelector<HTMLButtonElement>('[data-testid="durations"] button')?.click();
    await fixture.whenStable();

    expect(stub.calls).toEqual(['get 5', 'rent 5 1', 'get 5']);
    expect(text('message')).toBe('1725 rented for 1 day.');
    expect(text('status')).toBe('Rented');
  });

  it('says when there is no such machine', async () => {
    stub.answer = throwError(() => new HttpErrorResponse({ status: 404 }));

    await open('99');

    expect(text('missing')).toBe('There is no machine with that number.');
    expect(root().querySelector('app-equipment-card')).toBeNull();
  });
});
