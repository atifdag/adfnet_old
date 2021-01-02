import { TitleCasePipe } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AddModel } from 'src/app/models/add-model';
import { MenuModel } from 'src/app/models/menu-model';
import { GlobalizationDictionaryPipe } from 'src/app/pipes/globalization-dictionary.pipe';
import { GlobalizationMessagesPipe } from 'src/app/pipes/globalization-messages.pipe';
import { MainService } from 'src/app/services/main.service';
import { MenuService } from 'src/app/services/menu.service';
import { IdCodeName } from 'src/app/value-objects/id-code-name';
import { IdName } from 'src/app/value-objects/id-name';

@Component({
  selector: 'app-add',
  templateUrl: './add.component.html',
  styleUrls: ['./add.component.css']
})
export class AddComponent implements OnInit {

  model = new AddModel<MenuModel>();
  menuForm: FormGroup;
  submitted: boolean;
  loading = true;
  parentMenus:IdCodeName[]=[];
  parentId:string;
  constructor( private titlecasePipe: TitleCasePipe,
    private fb: FormBuilder,
    private router: Router,
    public globalizationDictionaryPipe: GlobalizationDictionaryPipe,
    public globalizationMessagesPipe: GlobalizationMessagesPipe,
    private serviceMenu: MenuService,
    private serviceMain: MainService,
    private route: ActivatedRoute,) { }

  ngOnInit(): void {
    this.fillModel();

    this.menuForm = this.fb.group({
      id: new FormControl({ disabled: true }, Validators.required),
      name: new FormControl('', [Validators.required, Validators.minLength(3)]),
      description: new FormControl('', []),
      code: new FormControl('', [Validators.required, Validators.minLength(2)]),
      address: new FormControl('', [Validators.required, Validators.minLength(2)]),
      parentMenus: new FormControl('', [Validators.required]),

      isApproved: new FormControl(true)
    });

  }

  
  fillModel(): void {
    this.serviceMenu.beforeAdd().subscribe(
      res => {
        if (res.status === 200) {
          this.model = res.body as AddModel<MenuModel>;

        } else {

          this.serviceMain.openErrorSnackBar('Kod: 01. ' + res.statusText);
        }
        this.loading = false;
      },
      err => {

        if (err.status === 400) {
          if (err.error != null) {
            this.model = err.error;
            const errors = Object.keys(err.error).map((t) => {
              return err.error[t];
            });
            this.serviceMain.openErrorSnackBar('Kod: 01. ' + errors.toString());
          } else {
            this.serviceMain.openErrorSnackBar('Kod: 01. ' + err.error);
          }
        }
        this.loading = false;
        // setTimeout(() => {
        //   this.router.navigate(['/User/List']);
        // }, 1000);
      }
    );

    this.getParentMenus();


  }

  parentChance()
  {
     this.model.item.parentMenu = this.parentMenus.find(p=>p.id == this.parentId ) ;


  }

  submit(): void {
    this.submitted = true;
    if (this.menuForm.invalid) {
      return;
    }
    this.loading = true;
    if (this.model.item == null) {
      this.model.item = new MenuModel();
    }
    console.log(this.model);
    this.serviceMenu.add(this.model).subscribe(
      res => {
        if (res.status === 200) {
          this.serviceMain.openSuccessSnackBar(this.globalizationMessagesPipe.transform('InfoSaveOperationSuccessful')
            + ' ' + this.globalizationDictionaryPipe.transform('RedirectionTitle'));
          setTimeout(() => {

            this.router.navigate(['/Menu/List']);
          }, 1000);
        } else {
          this.loading = false;
          this.serviceMain.openErrorSnackBar('Kod: 01. ' + res.statusText);
        }
      },
      err => {
        if (err.status === 400) {
          if (err.error != null) {
            this.loading = false;
            this.model = err.error;
            const errors = Object.keys(err.error).map((t) => {
              return err.error[t];
            });
            this.serviceMain.openErrorSnackBar('Kod: 01. ' + errors.toString());
          } else {
            this.loading = false;
            this.serviceMain.openErrorSnackBar('Kod: 01. ' + err.error);
          }
        }
      }
    );

  }

  globalizationMessages(key: string): string {
    return this.globalizationMessagesPipe.transform(key);
  }

  globalizationMessagesByParameter(key: string, parameter: string): string {
    return this.globalizationMessagesPipe.transform(key + ',' + parameter);
  }

  globalizationMessagesByParameter2(key: string, parameter1: string, parameter2: string): string {
    return this.globalizationMessagesPipe.transform(key + ',' + parameter1 + ',' + parameter2);
  }



  back(): void {
    this.router.navigate(['/Menu/List']);
  }

  getParentMenus(): void {
    this.serviceMenu.idNameList().subscribe(
      responseRole => {
        if (responseRole.status === 200) {
          const idNameList = responseRole.body as IdCodeName[];
          console.log(idNameList);
          if (idNameList.length > 0) {
            this.parentMenus = idNameList;
            //this.list();
          } else {
            this.serviceMain.openErrorSnackBar('Kod: 22. ' + responseRole.statusText);

          }
        } else {
          this.serviceMain.openErrorSnackBar('Kod: 23. ' + this.globalizationMessagesPipe.transform('DangerParentNotFound'));

        }
      },
      errorRole => {
        this.loading = false;
        if (errorRole.status === 400) {
          this.loading = false;
          this.serviceMain.openErrorSnackBar('Kod: 24. ' + this.globalizationMessagesPipe.transform('DangerParentNotFound'));

        }
        setTimeout(() => {
          this.router.navigate(['/Home']);
        }, 1000);
      }
    );
  }

}
