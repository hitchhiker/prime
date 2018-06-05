import { NgModule } from "@angular/core";
import { MatButtonModule, MatDialogContent, MatFormFieldModule, MatInputModule, MatAutocompleteModule } from "@angular/material";
import { MatToolbarModule } from "@angular/material";
import { MatTabsModule } from "@angular/material";
import { MatCardModule } from '@angular/material/card';
import { MatBadgeModule } from '@angular/material/badge';
import { MatDialogModule } from "@angular/material";
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatIconModule } from '@angular/material/icon';


@NgModule({
    imports:
        [
            MatButtonModule,
            MatToolbarModule,
            MatTabsModule,
            MatCardModule,
            MatBadgeModule,
            MatDialogModule,
            MatFormFieldModule,
            MatInputModule,
            MatSlideToggleModule,
            MatSnackBarModule,
            MatIconModule,
            MatAutocompleteModule
        ],
    exports:
        [
            MatButtonModule,
            MatToolbarModule,
            MatTabsModule,
            MatCardModule,
            MatBadgeModule,
            MatDialogModule,
            MatFormFieldModule,
            MatInputModule,
            MatSlideToggleModule,
            MatSnackBarModule,
            MatIconModule,
            MatAutocompleteModule
        ],
})
export class MaterialModule { }
