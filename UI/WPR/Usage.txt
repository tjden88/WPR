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
        Style="{StaticResource ChromeWindow}"
        Title="{Binding Title}" Height="650" Width="1000">
    <helpers:WindowHelper.WindowHeaderContent>
        <StackPanel Margin="3" Orientation="Horizontal" HorizontalAlignment="Left" Width="Auto" >
            <Button Content="?" Style="{StaticResource WindowTitleButton}"/>
        </StackPanel>
    </helpers:WindowHelper.WindowHeaderContent>

    <Grid>
        <TextBlock Text="Привет!" 
                   Foreground="{DynamicResource PrimaryColorBrush}"
                   FontSize="60"
                   HorizontalAlignment="Center" 
                   VerticalAlignment="Center"/>
    </Grid>
*/