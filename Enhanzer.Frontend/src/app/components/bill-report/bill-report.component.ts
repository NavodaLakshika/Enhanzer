import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { BillService, PurchaseBill } from '../../services/bill.service';

@Component({
  selector: 'app-bill-report',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatTableModule,
    MatIconModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './bill-report.component.html',
  styleUrl: './bill-report.component.scss'
})
export class BillReportComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private billService = inject(BillService);

  bill: PurchaseBill | null = null;
  isLoading = true;
  error = '';
  
  displayedColumns: string[] = ['itemName', 'batch', 'standardCost', 'qty', 'discount', 'totalCost', 'totalSelling'];

  ngOnInit() {
    const idParam = this.route.snapshot.paramMap.get('id');
    if (idParam) {
      const id = parseInt(idParam, 10);
      this.billService.getBill(id).subscribe({
        next: (data) => {
          this.bill = data;
          this.isLoading = false;
        },
        error: (err) => {
          console.error(err);
          this.error = 'Failed to load the bill report.';
          this.isLoading = false;
        }
      });
    } else {
      this.error = 'Invalid Bill ID.';
      this.isLoading = false;
    }
  }

  goBack() {
    this.router.navigate(['/purchase-bill']);
  }
}
