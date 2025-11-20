import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class LoadingService {
  private loadingSubject = new BehaviorSubject<boolean>(false);
  public loading$: Observable<boolean> = this.loadingSubject.asObservable();

  private requestCount = 0;
  private showTimeout: ReturnType<typeof setTimeout> | null = null;
  private readonly DELAY_MS = 100;

  show(): void {
    this.requestCount++;
    if (this.requestCount === 1) {
      this.showTimeout = setTimeout(() => {
        if (this.requestCount > 0) {
          this.loadingSubject.next(true);
        }
        this.showTimeout = null;
      }, this.DELAY_MS);
    }
  }

  hide(): void {
    this.requestCount--;

    if (this.showTimeout !== null) {
      clearTimeout(this.showTimeout);
      this.showTimeout = null;
    }

    if (this.requestCount <= 0) {
      this.requestCount = 0;
      this.loadingSubject.next(false);
    }
  }

  reset(): void {
    if (this.showTimeout !== null) {
      clearTimeout(this.showTimeout);
      this.showTimeout = null;
    }
    this.requestCount = 0;
    this.loadingSubject.next(false);
  }
}

