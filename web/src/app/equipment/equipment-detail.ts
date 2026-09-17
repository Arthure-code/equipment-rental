import { Component, effect, inject, input, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { Equipment } from './equipment';
import { EquipmentCard } from './equipment-card';
import { EquipmentService } from './equipment.service';

// One machine on its own page, reached by its id in the address. The
// same card as in the list does the renting.
@Component({
  selector: 'app-equipment-detail',
  imports: [EquipmentCard, RouterLink],
  templateUrl: './equipment-detail.html',
})
export class EquipmentDetail {
  private readonly service = inject(EquipmentService);

  readonly id = input.required<string>();
  readonly machine = signal<Equipment | null>(null);
  readonly missing = signal(false);
  readonly busy = signal(false);
  readonly message = signal<{ text: string; error: boolean } | null>(null);

  constructor() {
    effect(() => this.load(Number(this.id())));
  }

  rent(days: number): void {
    const machine = this.machine();
    if (!machine) return;
    this.busy.set(true);
    this.service.rent(machine.id, days).subscribe({
      next: () => {
        this.message.set({
          text: `${machine.name} rented for ${days} ${days === 1 ? 'day' : 'days'}.`,
          error: false,
        });
        this.load(machine.id);
      },
      error: () => this.fail(),
    });
  }

  cancel(): void {
    const machine = this.machine();
    if (!machine) return;
    this.busy.set(true);
    this.service.cancel(machine.id).subscribe({
      next: () => {
        this.message.set({ text: 'Rental cancelled.', error: false });
        this.load(machine.id);
      },
      error: () => this.fail(),
    });
  }

  private load(id: number): void {
    this.service.get(id).subscribe({
      next: (machine) => {
        this.machine.set(machine);
        this.busy.set(false);
      },
      error: () => {
        this.missing.set(true);
        this.busy.set(false);
      },
    });
  }

  private fail(): void {
    this.message.set({ text: 'Something went wrong. Please try again.', error: true });
    this.busy.set(false);
  }
}
