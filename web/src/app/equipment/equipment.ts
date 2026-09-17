// A machine as the API describes it. The dates come as ISO strings and
// exist only while the machine is out.
export interface Equipment {
  id: number;
  name: string;
  category: string;
  description: string;
  dailyRate: number;
  imageUrl: string;
  available: boolean;
  rentedFrom: string | null;
  rentedUntil: string | null;
}

export interface Rental {
  id: number;
  equipmentId: number;
  startsOn: string;
  endsOn: string;
  days: number;
  total: number;
}

// The durations a card offers, in days.
export const DURATIONS = [1, 3, 5, 7];
