import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { Observable, startWith, map } from 'rxjs';
import { BillService, LocationDetail, BillItem, PurchaseBill } from '../../services/bill.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-purchase-bill',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatAutocompleteModule,
    MatButtonModule,
    MatTableModule,
    MatIconModule,
    MatSnackBarModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './purchase-bill.component.html',
  styleUrl: './purchase-bill.component.scss'
})
export class PurchaseBillComponent implements OnInit {
  billForm: FormGroup;
  
  items: string[] = ['Mango', 'Apple', 'Banana', 'Orange', 'Grapes', 'Kiwi', 'Strawberry'];
  filteredItems!: Observable<string[]>;
  locations: LocationDetail[] = [];

  billItems: BillItem[] = [];
  displayedColumns: string[] = ['item', 'batch', 'standardCost', 'standardPrice', 'margin', 'qty', 'freeQty', 'discount', 'totalCost', 'totalSelling'];

  private fb = inject(FormBuilder);
  private billService = inject(BillService);
  private snackBar = inject(MatSnackBar);
  private authService = inject(AuthService);
  private router = inject(Router);

  isSaving = false;

  constructor() {
    this.billForm = this.fb.group({
      item: ['', Validators.required],
      batch: ['', Validators.required],
      standardCost: [null, [Validators.required, Validators.min(0)]],
      standardPrice: [null, [Validators.required, Validators.min(0)]],
      margin: [null],
      qty: [1, [Validators.required, Validators.min(1)]],
      freeQty: [null],
      discount: [null, [Validators.min(0), Validators.max(100)]]
    });
  }

  ngOnInit() {
    this.billService.getLocations().subscribe({
      next: (data) => this.locations = data,
      error: (err) => console.error('Failed to load locations', err)
    });

    this.filteredItems = this.billForm.get('item')!.valueChanges.pipe(
      startWith(''),
      map(value => this._filter(value || ''))
    );
  }

  private _filter(value: string): string[] {
    const filterValue = value.toLowerCase();
    return this.items.filter(item => item.toLowerCase().includes(filterValue));
  }

  get totalCostValue(): number {
    const standardCost = Number(this.billForm.value.standardCost || 0);
    const qty = Number(this.billForm.value.qty || 0);
    const discount = Number(this.billForm.value.discount || 0);
    const baseCost = standardCost * qty;
    const discountAmt = baseCost * (discount / 100);
    return baseCost - discountAmt;
  }

  get totalSellingValue(): number {
    const standardPrice = Number(this.billForm.value.standardPrice || 0);
    const qty = Number(this.billForm.value.qty || 0);
    return standardPrice * qty;
  }

  get totalItemsCount(): number {
    return this.billItems.length;
  }

  get totalQuantityCount(): number {
    return this.billItems.reduce((acc, item) => acc + Number(item.qty) + Number(item.freeQty || 0), 0);
  }

  onAdd() {
    if (this.billForm.invalid) {
      this.billForm.markAllAsTouched();
      this.snackBar.open('Please fill in all required fields correctly.', 'Close', {
        duration: 3000,
        panelClass: ['error-snackbar']
      });
      return;
    }

    const formVal = this.billForm.value;
    
    const newItem: BillItem = {
      itemName: formVal.item,
      batch: formVal.batch,
      standardCost: Number(formVal.standardCost || 0),
      standardPrice: Number(formVal.standardPrice || 0),
      margin: Number(formVal.margin || 0),
      qty: Number(formVal.qty || 0),
      freeQty: Number(formVal.freeQty || 0),
      discount: Number(formVal.discount || 0),
      totalCost: this.totalCostValue,
      totalSelling: this.totalSellingValue
    };

    this.billItems = [...this.billItems, newItem];
    
    this.billForm.reset({
      item: '',
      batch: '',
      standardCost: null,
      standardPrice: null,
      margin: null,
      qty: 1,
      freeQty: null,
      discount: null
    });

    this.snackBar.open('Item successfully added to the bill!', 'Close', {
      duration: 3000,
      panelClass: ['success-snackbar']
    });
  }

  saveBill() {
    if (this.billItems.length === 0) {
      this.snackBar.open('Please add at least one item before saving.', 'Close', { duration: 3000, panelClass: ['error-snackbar'] });
      return;
    }

    this.isSaving = true;
    
    const purchaseBill: PurchaseBill = {
      totalItems: this.totalItemsCount,
      totalQuantity: this.totalQuantityCount,
      totalCost: this.billItems.reduce((acc, item) => acc + item.totalCost, 0),
      totalSelling: this.billItems.reduce((acc, item) => acc + item.totalSelling, 0),
      items: this.billItems
    };

    this.billService.saveBill(purchaseBill).subscribe({
      next: (savedBill) => {
        this.isSaving = false;
        this.snackBar.open('Purchase Bill saved successfully!', 'Close', { duration: 3000, panelClass: ['success-snackbar'] });
        this.router.navigate(['/bill-report', savedBill.id]);
      },
      error: (err) => {
        this.isSaving = false;
        console.error('Error saving bill', err);
        this.snackBar.open('Failed to save bill. Please try again.', 'Close', { duration: 3000, panelClass: ['error-snackbar'] });
      }
    });
  }

  onLogout() {
    this.authService.logout();
  }
}
