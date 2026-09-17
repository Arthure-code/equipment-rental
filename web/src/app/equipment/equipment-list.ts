import { Component, computed, inject, signal } from '@angular/core';
import { Equipment } from './equipment';
import { EquipmentCard } from './equipment-card';
import { EquipmentService } from './equipment.service';

const ALL = 'All';

// The catalogue: every machine as a card, filtered by category, with a
// message for what just happened.
@Component({
  selector: 'app-equipment-list',
  imports: [EquipmentCard],
  templateUrl: './equipment-list.html',
  styleUrl: './equipment-list.css',
})
export class EquipmentList {
  private readonly service = inject(EquipmentService);

  readonly fleet = signal<Equipment[]>([]);
  readonly category = signal(ALL);
  readonly loading = signal(true);
  readonly busyId = signal<number | null>(null);
  readonly message = signal<{ text: string; error: boolean } | null>(null);

  // Six empty cards keep the page's shape while the fleet loads.
  readonly placeholders = [1, 2, 3, 4, 5, 6];

  readonly categories = computed(() => [ALL, ...new Set(this.fleet().map((e) => e.category))]);
  readonly shown = computed(() =>
    this.category() === ALL
      ? this.fleet()
      : this.fleet().filter((e) => e.category === this.category()),
  );
  readonly availableCount = computed(() => this.shown().filter((e) => e.available).length);

  constructor() {
    this.load();
  }

  rent(machine: Equipment, days: number): void {
    this.busyId.set(machine.id);
    this.service.rent(machine.id, days).subscribe({
      next: (rental) => {
        this.say(
          `${machine.name} rented for ${days} ${days === 1 ? 'day' : 'days'}, ${money(rental.total)}.`,
        );
        this.load();
      },
      error: (error) =>
        this.fail(error.status === 409 ? `${machine.name} is already rented.` : undefined),
    });
  }

  cancel(machine: Equipment): void {
    this.busyId.set(machine.id);
    this.service.cancel(machine.id).subscribe({
      next: () => {
        this.say(`Rental of ${machine.name} cancelled.`);
        this.load();
      },
      error: () => this.fail(),
    });
  }

  private load(): void {
    this.service.getAll().subscribe({
      next: (fleet) => {
        this.fleet.set(fleet);
        this.loading.set(false);
        this.busyId.set(null);
      },
      error: () => this.fail('The rental service did not answer. Check that the API is running.'),
    });
  }

  private say(text: string): void {
    this.message.set({ text, error: false });
  }

  private fail(text = 'Something went wrong. Please try again.'): void {
    this.message.set({ text, error: true });
    this.loading.set(false);
    this.busyId.set(null);
  }
}

function money(amount: number): string {
  return new Intl.NumberFormat('en-CA', {
    style: 'currency',
    currency: 'CAD',
    maximumFractionDigits: 0,
  }).format(amount);
}
