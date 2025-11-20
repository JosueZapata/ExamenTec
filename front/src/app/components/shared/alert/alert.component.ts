import { Component, Input, OnInit, OnDestroy, Output, EventEmitter, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';

export type AlertType = 'success' | 'error';

@Component({
  selector: 'app-alert',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './alert.component.html',
  styleUrl: './alert.component.scss'
})
export class AlertComponent implements OnInit, OnChanges, OnDestroy {
  @Input() type: AlertType = 'error';
  @Input() message: string = '';
  @Input() duration: number = 5000;
  @Input() showCloseButton: boolean = true;
  @Output() closed = new EventEmitter<void>();

  isVisible = false;
  private timeoutId?: number;

  ngOnInit(): void {
    if (this.message) {
      this.show();
    }
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['message'] && this.message) {
      this.show();
    } else if (changes['message'] && !this.message) {
      this.close();
    }
  }

  ngOnDestroy(): void {
    this.clearTimeout();
  }

  show(): void {
    this.isVisible = true;
    this.clearTimeout();
    
    if (this.duration > 0) {
      this.timeoutId = window.setTimeout(() => {
        this.close();
      }, this.duration);
    }
  }

  close(): void {
    this.isVisible = false;
    this.clearTimeout();
    this.closed.emit();
  }

  private clearTimeout(): void {
    if (this.timeoutId) {
      clearTimeout(this.timeoutId);
      this.timeoutId = undefined;
    }
  }

  get alertClasses(): string {
    const baseClasses = 'mb-4 px-4 py-3 rounded flex items-center justify-between';
    
    if (this.type === 'success') {
      return `${baseClasses} bg-green-100 border border-green-400 text-green-700`;
    }
    
    return `${baseClasses} bg-red-100 border border-red-400 text-red-700`;
  }

  get iconClasses(): string {
    if (this.type === 'success') {
      return 'text-green-600';
    }
    return 'text-red-600';
  }
}

