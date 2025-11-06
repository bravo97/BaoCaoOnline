import { Component, ViewEncapsulation } from '@angular/core';
import { Sidebar } from "../../layout/sidebar/sidebar";
import { Header } from "../../layout/header/header";

@Component({
  selector: 'app-setting',
  standalone:true,
  imports: [Sidebar, Header],
  templateUrl: './setting.html',
  styleUrls: [
    '../../admin.scss',
    './setting.scss'],
  encapsulation: ViewEncapsulation.None
})
export class Setting {
  headerTitle = 'Thiết lập hệ thống';
  onMenuSelected(menu: string) {
      this.headerTitle = menu;
    }
}
