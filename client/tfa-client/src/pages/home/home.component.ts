import { ChangeDetectionStrategy, Component } from "@angular/core";

@Component({
    selector: 'tfa-home',
    templateUrl: './home.component.html',
    standalone: true,
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: []
})
export class HomeComponent {

}
