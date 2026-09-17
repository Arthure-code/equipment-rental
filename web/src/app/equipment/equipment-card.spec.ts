import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { EquipmentCard } from './equipment-card';
import { Equipment } from './equipment';

const loader: Equipment = {
  id: 3,
  name: '744 P-Tier',
  category: 'Earthmoving',
  description: 'Wheel loader.',
  dailyRate: 1100,
  imageUrl: 'https://images.unsplash.com/photo-1751054786365-4b02b690d301',
  available: true,
  rentedFrom: null,
  rentedUntil: null,
};

describe('EquipmentCard', () => {
  let fixture: ComponentFixture<EquipmentCard>;

  const root = () => fixture.nativeElement as HTMLElement;
  const text = (testId: string) =>
    root().querySelector(`[data-testid="${testId}"]`)?.textContent?.replace(/\s+/g, ' ').trim();

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EquipmentCard],
      providers: [provideRouter([])],
    }).compileComponents();
    fixture = TestBed.createComponent(EquipmentCard);
    fixture.componentRef.setInput('equipment', loader);
    fixture.autoDetectChanges();
    await fixture.whenStable();
  });

  it('shows the photo, the category, the name and the daily rate', () => {
    const img = root().querySelector('img');
    expect(img?.getAttribute('src')).toBe(
      `${loader.imageUrl}?auto=format&fit=crop&w=640&h=400&q=60`,
    );
    expect(img?.getAttribute('srcset')).toContain('w=400&h=250&q=60 400w');
    expect(img?.getAttribute('alt')).toBe('744 P-Tier, earthmoving');
    expect(root().textContent).toContain('Earthmoving');
    expect(root().querySelector('h2')?.textContent?.trim()).toBe('744 P-Tier');
    expect(text('rate')).toBe('$1,100 / day');
    expect(text('status')).toBe('Available');
  });

  it('offers four durations, each priced, and emits the one clicked', () => {
    const buttons = root().querySelectorAll<HTMLButtonElement>('[data-testid="durations"] button');
    const emitted: number[] = [];
    fixture.componentInstance.rent.subscribe((days) => emitted.push(days));

    expect([...buttons].map((b) => b.textContent?.replace(/\s+/g, ' ').trim())).toEqual([
      '1 day $1,100',
      '3 days $3,300',
      '5 days $5,500',
      '7 days $7,700',
    ]);
    buttons[1].click();
    expect(emitted).toEqual([3]);
  });

  it('shows until when a rented machine is out and emits the cancellation', async () => {
    fixture.componentRef.setInput('equipment', {
      ...loader,
      available: false,
      rentedFrom: '2026-09-17T18:00:00Z',
      rentedUntil: '2026-09-20T18:00:00Z',
    });
    await fixture.whenStable();
    let cancelled = false;
    fixture.componentInstance.cancelRental.subscribe(() => (cancelled = true));

    expect(text('status')).toBe('Rented');
    expect(text('rented-until')).toBe('Rented until Sep 20, 2026');
    expect(root().querySelector('[data-testid="durations"]')).toBeNull();
    root().querySelector<HTMLButtonElement>('button.btn-outline-secondary')?.click();
    expect(cancelled).toBe(true);
  });

  it('disables its buttons while a request is in flight', async () => {
    fixture.componentRef.setInput('busy', true);
    await fixture.whenStable();

    const buttons = root().querySelectorAll<HTMLButtonElement>('button');
    expect(buttons.length).toBe(4);
    expect([...buttons].every((b) => b.disabled)).toBe(true);
  });
});
