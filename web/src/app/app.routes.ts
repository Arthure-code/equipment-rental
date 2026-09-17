import { Routes } from '@angular/router';
import { EquipmentList } from './equipment/equipment-list';
import { EquipmentDetail } from './equipment/equipment-detail';

export const routes: Routes = [
  { path: '', redirectTo: 'equipment', pathMatch: 'full' },
  { path: 'equipment', component: EquipmentList },
  { path: 'equipment/:id', component: EquipmentDetail },
  { path: '**', redirectTo: 'equipment' },
];
