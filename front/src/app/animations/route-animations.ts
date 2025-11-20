import { trigger, transition, style, query, animate } from '@angular/animations';

export const slideInAnimation = trigger('routeAnimations', [
  transition('* <=> *', [
    query(':enter, :leave', [
      style({
        position: 'absolute',
        left: 0,
        right: 0,
        width: '100%',
        opacity: 0,
        transform: 'translateY(20px) scale(0.98)'
      })
    ], { optional: true }),
    query(':enter', [
      animate('350ms cubic-bezier(0.4, 0.0, 0.2, 1)', style({
        opacity: 1,
        transform: 'translateY(0) scale(1)'
      }))
    ], { optional: true })
  ])
]);

