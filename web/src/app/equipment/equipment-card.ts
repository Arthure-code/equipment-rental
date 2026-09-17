import { CurrencyPipe, DatePipe } from '@angular/common';
import { Component, computed, input, output } from '@angular/core';
import { RouterLink } from '@angular/router';
import { DURATIONS, Equipment } from './equipment';

// One machine: its photo, its card, and what can be done with it right
// now, rent for a few days or cancel the rental in progress.
@Component({
  selector: 'app-equipment-card',
  imports: [CurrencyPipe, DatePipe, RouterLink],
  templateUrl: './equipment-card.html',
  styleUrl: './equipment-card.css',
})
export class EquipmentCard {
  readonly equipment = input.required<Equipment>();
  readonly busy = input(false);
  // The first cards are above the fold: their photos load first, the
  // others wait until they are near the screen.
  readonly priority = input(false);
  readonly rent = output<number>();
  readonly cancelRental = output<void>();

  readonly durations = DURATIONS;

  // Unsplash resizes on request: one URL per width, the browser picks.
  readonly photo = computed(() => photoSources(this.equipment().imageUrl));
}

const WIDTHS = [400, 640, 800];

export function photoSources(url: string): { src: string; srcset: string } {
  const at = (width: number) =>
    `${url}?auto=format&fit=crop&w=${width}&h=${Math.round(width * 0.625)}&q=60`;
  return {
    src: at(640),
    srcset: WIDTHS.map((width) => `${at(width)} ${width}w`).join(', '),
  };
}
