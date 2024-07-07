import { ApplicationConfig, EnvironmentProviders, Provider, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { HTTP_INTERCEPTORS, provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { MAT_FORM_FIELD_DEFAULT_OPTIONS } from '@angular/material/form-field';
import { DefaultHttpHeaderInterceptor } from '../data/http/http.interceptor';

const provideMaterialElements = (): (Provider | EnvironmentProviders)[] => {
    return [
        {provide: MAT_FORM_FIELD_DEFAULT_OPTIONS, useValue: { floatLabel: 'always', appearance: 'outline' }}
    ]
}

export const appConfig: ApplicationConfig = {
    providers: [
        provideZoneChangeDetection({ eventCoalescing: true }),
        provideRouter(routes),
        provideHttpClient(withInterceptorsFromDi()),
        {
            provide: HTTP_INTERCEPTORS,
            useClass: DefaultHttpHeaderInterceptor,
            multi: true
        },
        provideAnimationsAsync(),
        ...provideMaterialElements(),
    ]
};
