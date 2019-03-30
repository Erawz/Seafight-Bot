namespace BoxyBot.Properties {
    
    
    // Cette classe vous permet de gérer certains événements de la classe de préférence:
    //  L'événement SettingChanging se déclenche avant de modifier la valeur d'un paramètre.
    //  L'événement PropertyChanged est déclenché après la modification de la valeur d'un paramètre.
    //  L'événement SettingsLoaded se déclenche après le chargement des valeurs de réglage.
    //  L'événement SettingsSaving est déclenché avant que les valeurs de paramètres ne soient enregistrées.
    internal sealed partial class Settings {
        
        public Settings() {
            // // Décommentez les lignes ci-dessous pour ajouter des gestionnaires d'événements permettant de sauvegarder et de modifier les paramètres:
            //
            // this.SettingChanging += this.SettingChangingEventHandler;
            //
            // this.SettingsSaving += this.SettingsSavingEventHandler;
            //
        }
        
        private void SettingChangingEventHandler(object sender, System.Configuration.SettingChangingEventArgs e) {
            // Ajoutez du code pour gérer l'événement SettingChangingEvent.
        }
        
        private void SettingsSavingEventHandler(object sender, System.ComponentModel.CancelEventArgs e) {
            // Ajoutez du code pour gérer l'événement SettingsSaving ici.
        }
    }
}
