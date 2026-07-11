import { Routes } from '@angular/router';
import { LoginComponent } from './components/login/login.component';
import { PurchaseBillComponent } from './components/purchase-bill/purchase-bill.component';
import { BillReportComponent } from './components/bill-report/bill-report.component';
import { authGuard } from './guards/auth.guard';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'purchase-bill', component: PurchaseBillComponent, canActivate: [authGuard] },
  { path: 'bill-report/:id', component: BillReportComponent, canActivate: [authGuard] },
  { path: '', redirectTo: '/purchase-bill', pathMatch: 'full' },
  { path: '**', redirectTo: '/login' }
];
