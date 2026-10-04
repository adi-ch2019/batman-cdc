import { Component, signal, inject } from '@angular/core';
import { DatePipe } from '@angular/common';
import { BatmanService, BatmanItem } from '../services/batman.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [DatePipe],
  template: `
    <div class="dashboard">
      <h2>🦇 BATMAN CDC Dashboard</h2>
      
      <div class="actions">
        <button (click)="refresh()" [disabled]="loading()">
          {{ loading() ? 'Loading...' : 'Refresh' }}
        </button>
        
        <div class="create-form">
          <input 
            #nameInput
            type="text" 
            placeholder="Item name"
            (keyup.enter)="createItem(nameInput.value)"
          />
          <button (click)="createItem(nameInput.value)">
            Create
          </button>
        </div>
      </div>

      @if (items().length === 0) {
        <p class="empty">No items. Create one to trigger the CDC stream!</p>
      } @else {
        <div class="item-list">
          @for (item of items(); track item.id) {
            <div class="item-card">
              <h3>{{ item.name }}</h3>
              <p class="meta">
                ID: {{ item.id }}<br>
                Modified: {{ item.lastModified | date:'medium' }}
              </p>
            </div>
          }
        </div>
      }
    </div>
  `,
  styles: [`
    .dashboard { padding: 2rem; font-family: system-ui; max-width: 800px; margin: 0 auto; }
    .actions { display: flex; gap: 1rem; margin-bottom: 2rem; flex-wrap: wrap; }
    .create-form { display: flex; gap: 0.5rem; }
    input { padding: 0.5rem; border: 1px solid #ccc; border-radius: 4px; }
    button { padding: 0.5rem 1rem; background: #1a1a2e; color: white; 
             border: none; border-radius: 4px; cursor: pointer; }
    button:disabled { opacity: 0.5; cursor: not-allowed; }
    .item-list { display: grid; gap: 1rem; }
    .item-card { padding: 1rem; border: 1px solid #eee; border-radius: 8px; 
                 background: #fafafa; }
    .item-card h3 { margin: 0 0 0.5rem; color: #1a1a2e; }
    .meta { font-size: 0.85rem; color: #666; margin: 0; }
    .empty { color: #999; font-style: italic; }
  `]
})
export class DashboardComponent {
  private batmanService = inject(BatmanService);
  
  items = signal<BatmanItem[]>([]);
  loading = signal(false);

  constructor() {
    this.refresh();
  }

  refresh(): void {
    this.loading.set(true);
    this.batmanService.getItems().subscribe({
      next: (data) => {
        this.items.set(data);
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Failed to load items:', err);
        this.loading.set(false);
      }
    });
  }

  createItem(name: string): void {
    if (!name?.trim()) return;
    
    this.batmanService.createItem(name.trim()).subscribe({
      next: () => {
        this.refresh(); // Reload to see the new item
      },
      error: (err) => console.error('Failed to create:', err)
    });
  }
}