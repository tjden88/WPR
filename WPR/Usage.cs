// App.xaml:
/* 
<ResourceDictionary.MergedDictionaries>
    <ResourceDictionary Source = "pack://application:,,,/WPR;component/Styles.xaml"/>
</ResourceDictionary.MergedDictionaries>

//Window:

------ >>>

        xmlns:wpr="clr-namespace:WPR;assembly=WPR"
        xmlns:cnt="clr-namespace:WPR.Controls;assembly=WPR"
        xmlns:helpers="clr-namespace:WPR.Helpers;assembly=WPR"
        Style="{StaticResource ChromeWindow}" helpers:WindowHelper.TitleLeftMargin ="{Binding ElementName=TitleButtonsPanel, Path=ActualWidth}"
        Title="{Binding Title}" Height="650" Width="1000">
    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height="35"/>
            <RowDefinition/>
        </Grid.RowDefinitions>
        <!-- Панель в заголовке для кнопок-->
        <StackPanel Margin="3" x:Name="TitleButtonsPanel" Orientation="Horizontal" HorizontalAlignment="Left" Width="Auto">
        </StackPanel>
        <!--Основной контент в 1 строке грида-->
        <Grid Grid.Row="1"
              Background="{DynamicResource BackgroundColorBrush}">
            <TextBlock Text="Привет!" 
                       Foreground="{DynamicResource PrimaryColorBrush}"
                       FontSize="60"
                       HorizontalAlignment="Center" 
                       VerticalAlignment="Center"/>
        </Grid>
    </Grid>
*/