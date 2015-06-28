Imports Microsoft.Win32
Imports System.IO
Imports System
Imports System.ComponentModel
Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Threading

Public Class MainForm
    Dim InfoThread As Thread
    Private Delegate Sub voidDelegate(ByRef ServerVersion As String, ServerPlayerCount As String)

    Private Sub Main_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        LauncherUpdate.Close()
        iniSub(0)


        If Int(My.Computer.Info.AvailablePhysicalMemory / 1024 / 1024) > 4608 Then
            TextBoxAvailableMem.Text = 4096
        Else
            TextBoxAvailableMem.Text = Int(My.Computer.Info.AvailablePhysicalMemory / 1024 / 1024 * 0.9)
        End If

        TimerRefreshInfo_Tick()
        TimerRefreshInfo.Enabled = True

        InfoThread = New Thread(New ThreadStart(AddressOf ServerInfo))
        InfoThread.Start()

    End Sub

    Private Sub ServerInfo()
        'Get Server Info
        Dim ServerVersion As String, ServerPlayerCount As String

        Try

            Dim ip As String = ""
            Dim ipHost As IPHostEntry = Dns.GetHostEntry("mc.ime.moe")
            For Each ip1 As IPAddress In ipHost.AddressList
                ip = ip1.ToString
                Exit For
            Next
            Dim MCServerInfo As eMZi.Gaming.Minecraft.MinecraftServerInfo
            Dim MCServerIPAddress As IPAddress
            MCServerIPAddress = IPAddress.Parse(ip)
            Dim MCServerPoint As New IPEndPoint(MCServerIPAddress, 25565)

            MCServerInfo = eMZi.Gaming.Minecraft.MinecraftServerInfo.GetServerInformation(MCServerPoint)
            ServerVersion = "Server Version:" & MCServerInfo.MinecraftVersion
            ServerPlayerCount = "Current Player:" & MCServerInfo.CurrentPlayerCount & "/" & MCServerInfo.MaxPlayerCount
        Catch ex As Sockets.SocketException
            ServerVersion = "Server Version:Error"
            ServerPlayerCount = "Current Player:Error"
        End Try
        Me.Invoke(New voidDelegate(AddressOf UpdateUI), ServerVersion, ServerPlayerCount)
    End Sub

    Private Sub UpdateUI(ByRef ServerVersion As String, ServerPlayerCount As String)
        LabelServerVersion.Text = ServerVersion
        LabelServerPlayerCount.Text = ServerPlayerCount
    End Sub


    Private Sub TimerAutoKill_Tick(sender As Object, e As EventArgs) Handles TimerAutoKill.Tick
        Process.GetCurrentProcess().Kill()
    End Sub


#Region "Set Reg"
    Private Sub SetReg()

        If Registry.CurrentUser.OpenSubKey("SOFTWARE\LightMCLauncher", True) Is Nothing Then
            Registry.CurrentUser.CreateSubKey("Software\LightMCLauncher")
        End If
        Registry.CurrentUser.OpenSubKey("SOFTWARE\LightMCLauncher", True).SetValue("Username", tbxUsername.Text)
        Registry.CurrentUser.OpenSubKey("SOFTWARE\LightMCLauncher", True).SetValue("Parameter", TextBoxParameter.Text)

    End Sub
#End Region

#Region "RunMC"
    Private Sub RunMc()
        Dim dirAimPathTemp As New IO.DirectoryInfo(Application.StartupPath & "\.minecraft\versions\"), strForgeVersion As String
        strForgeVersion = dirAimPathTemp.GetDirectories.GetValue(0).ToString
        Dim swRunMcBat As New IO.StreamWriter(Environment.GetEnvironmentVariable("temp") & "\runmc.bat", False, System.Text.Encoding.Default)
        'Dim swRunMcVbs As New IO.StreamWriter(Environment.GetEnvironmentVariable("temp") & "\runmc.vbs", False, System.Text.Encoding.Default)
        'swRunMcBat.WriteLine(CoreFunction.Core(0, TextBoxAvailableMem.Text, strForgeVersion, TextBoxUsername.Text, TextBoxParameter.Text) & Chr(13) + Chr(10) & "pause")
        'swRunMcBat.Close()
        'swRunMcVbs.WriteLine("set ws=wscript.createobject(" & """" & "wscript.shell" & """" & ") " & Chr(13) + Chr(10) & "ws.run " & """" & Environment.GetEnvironmentVariable("temp") & "\runmc.bat" & """" & ",0")
        swRunMcBat.WriteLine(tbxJava.Text + PeoLeser.Minecraft.CoreClass.Core(0, TextBoxAvailableMem.Text, strForgeVersion, tbxUsername.Text, TextBoxParameter.Text))
        'swRunMcVbs.Close()
        swRunMcBat.Close()
        ' Process.Start(Environment.GetEnvironmentVariable("temp") & "\runmc.vbs")
        Process.Start(tbxJava.Text, PeoLeser.Minecraft.CoreClass.Core(0, TextBoxAvailableMem.Text, strForgeVersion, tbxUsername.Text, TextBoxParameter.Text))
    End Sub
#End Region

#Region "get java path"
    Private Function GetJavaHome()
        Try
            Dim strJavaHome As String, strJavaVer As String
            strJavaVer = Registry.LocalMachine.OpenSubKey("SOFTWARE\javasoft\Java Runtime Environment", True).GetValue("CurrentVersion")
            strJavaVer = Registry.LocalMachine.OpenSubKey("SOFTWARE\javasoft\Java Runtime Environment\" & strJavaVer, True).GetValue("JavaHome")
            strJavaHome = strJavaVer & "\bin\java.exe"
            Return strJavaHome
        Catch NoJava As Exception
            MessageBox.Show("Cannot find Java.exe!")
        End Try
    End Function
#End Region



    Private Sub TimerRefreshInfo_Tick() Handles TimerRefreshInfo.Tick
        'get MEM
        LabelTotalMemNum.Text = Int(My.Computer.Info.TotalPhysicalMemory / 1024 / 1024) & " M"
        LabelAvailableMemNum.Text = Int(My.Computer.Info.AvailablePhysicalMemory / 1024 / 1024) & " M"

    End Sub


    Private Sub iniSub(ByVal SubMode As Integer)
        'Dim iniPath As String = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\LightMCLauncher\user.ini"
        Dim iniPath As String = Application.StartupPath + "\LightMCLauncher\user.ini"
        If Directory.Exists(Application.StartupPath + "\LightMCLauncher") = False Then Directory.CreateDirectory(Application.StartupPath + "\LightMCLauncher")

        Select Case SubMode
            Case 0
                Try
                    tbxUsername.Text = PeoLeser.Minecraft.CoreClass.GetINI("User", "Username", "", iniPath)
                    TextBoxParameter.Text = PeoLeser.Minecraft.CoreClass.GetINI("User", "Parameter", "", iniPath)
                    tbxJava.Text = PeoLeser.Minecraft.CoreClass.GetINI("User", "Java", "", iniPath)
                Catch ex As Exception
                End Try
                If tbxJava.Text = "" Then
                    Try
                        tbxJava.Text = My.Computer.Registry.GetValue("HKEY_LOCAL_MACHINE\SOFTWARE\Classes\jarfile\shell\open\command", "", Nothing).ToString.Split(Chr(34))(1)
                    Catch ex As Exception
                        tbxJava.Text = "Pleaset set java path!"
                    End Try
                End If
                If tbxUsername.Text = "" Then tbxUsername.Text = "请输入昵称"
            Case 1
                PeoLeser.Minecraft.CoreClass.WriteINI("User", "Username", tbxUsername.Text, iniPath)
                PeoLeser.Minecraft.CoreClass.WriteINI("User", "Parameter", TextBoxParameter.Text, iniPath)
                PeoLeser.Minecraft.CoreClass.WriteINI("User", "Java", tbxJava.Text, iniPath)
        End Select
    End Sub


    Private Sub ContextMenuStrip1_Opening(sender As Object, e As CancelEventArgs) Handles ContextMenuStrip1.Opening

    End Sub

    Private Sub LabelTotalMemNum_Click(sender As Object, e As EventArgs) Handles LabelTotalMemNum.Click

    End Sub

    Private Sub LabelAvailableMemNum_Click(sender As Object, e As EventArgs) Handles LabelAvailableMemNum.Click

    End Sub


    Private Sub ButtonRunMc_Click_1(sender As Object, e As EventArgs) Handles ButtonRunMc.Click
        Try
            Shell(tbxJava.Text, AppWinStyle.Hide)
            iniSub(1)
            'Shell(GetMcPath(), AppWinStyle.NormalNoFocus)
            MinecraftUpdate.ShowDialog()
            RunMc()
            Application.Exit()
        Catch NoJava As FileNotFoundException
            MessageBox.Show("Cannot find java!")
        End Try
    End Sub

    Private Sub ButtonDefaultParameter_Click_1(sender As Object, e As EventArgs) Handles ButtonDefaultParameter.Click
        MessageBox.Show("-XX:-UseVMInterruptibleIO -XX:NewRatio=3 -XX:+UseStringCache -XX:+UseConcMarkSweepGC -XX:+UseParNewGC -XX:+CMSIncrementalPacing -XX:+AggressiveOpts -XX:+UseFastAccessorMethods -XX:+UseBiasedLocking -XX:PermSize=128m -XX:MaxPermSize=256m -XX:+CMSParallelRemarkEnabled -XX:MaxGCPauseMillis=50 -XX:+UseAdaptiveGCBoundary -XX:-UseGCOverheadLimit -XX:SurvivorRatio=8 -XX:TargetSurvivorRatio=90 -XX:MaxTenuringThreshold=15 -XX:+UseAdaptiveSizePolicy -XX:+DisableExplicitGC -Xnoclassgc -oss4M -ss4M -XX:CMSInitiatingOccupancyFraction=60 -XX:+UseCMSCompactAtFullCollection -XX:CMSFullGCsBeforeCompaction=1 -XX:SoftRefLRUPolicyMSPerMB=2048 -Xms800M -XX:ParallelGCThreads=" & System.Environment.ProcessorCount & " -Dfml.ignoreInvalidMinecraftCertificates=true -Dfml.ignorePatchDiscrepancies=true ")
    End Sub

    Private Sub btnJava_Click(sender As Object, e As EventArgs) Handles btnJava.Click
        OpenFileDialogJava.ShowDialog()
        tbxJava.Text = OpenFileDialogJava.FileName.ToString
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub OpenFileDialogJava_FileOk(sender As Object, e As CancelEventArgs) Handles OpenFileDialogJava.FileOk

    End Sub

    Private Sub RandomUser_Click_1(sender As Object, e As EventArgs) Handles RandomUser.Click
        Dim AllUser As String() = "Aabbye#Aaron#Abagael#Abagail#Abbe#Abbey#Abbi#Abbie#Abbott#Abbra#Abby#Abdul#Abe#Abel#Abelard#Abeni#Abia#Abiba#Abie#Abigael#Abigail#Abigale#Abner#Abra#Abraham#Abram#Abrams#Abrienda#Abril#Absinthia#Abu#Acacia#Ace#Ada#Adah#Adair#Adalia#Adam#Adamina#Adamma#Adan#Adara#Adda#Addi#Addia#Addie#Addison#Addy#Ade#Adela#Adelaide#Adele#Adeline#Adelle#Adem#Aden#Adena#Aderes#Adey#Adi#Adila#Adina#Adita#Adlai#Adler#Adli#Adolph#Adonai#Adonia#Adora#Adrian#Adrienne#Ady#Aelan#Aelene#Afton#Agatha#Agnes#Ahmed#Aida#Aidan#Aila#Ailee#Aileen#Ailene#Ailey#Aili#Ailis#Aimee#Aine#Ainslee#Ainsley#Ainslie#Airelle#Aislin#Ajani#Akili#Al#Alair#Alameda#Alan#Alana#Alanna#Alastair#Albany#Albert#Alberta#Alberto#Alden#Aldous#Aldrich#Alec#Aleda#Aleen#Alem#Alene#Alenna#Alessa#Aletha#Alex#Alexa#Alexander#Alexandra#Alexandria#Alexia#Alexis#Alfi#Alfie#Alfred#Alfreda#Alfy#Alger#Ali#Alia#Alice#Alicia#Alida#Alika#Alima#Alina#Aline#Alish#Alisha#Alison#Alissa#Aliya#Alize#Alka#Allan#Allard#Allegra#Allen#Allene#Allie#Allison#Alma#Almeda#Almeta#Alonzo#Alphonse#Althea#Alva#Alvin#Alvina#Alvinia#Alyn#Alyssa#Amabel#Amabelle#Amador#Amalie#Aman#Amana#Amanad#Amanda#Amandi#Amandie#Amandy#Amara#Amarante#Amaris#Amaya#Amberann#Ambrose#Amelia#Amena#Ami#Amiel#Amina#Amir#Amity#Amma#Amorin#Amory#Amos#Amy#An#Ana#Anabel#Anabella#Anabelle#Anais#Analiese#Analise#Anana#Ananda#Anastasia#Anatola#Anatole#Ande#Anders#Andie#Andra#Andralyn#Andre#Andrea#Andrew#Andy#Anet#Anett#Anette#Ange#Angel#Angela#Angele#Angelica#Angelika#Angelina#Angelinea#Angelique#Angelita#Angelle#Angelo#Angie#Angil#Angus#Angy#Ania#Anica#Anika#Anisa#Anissa#Anita#Anitra#Anja#Anliese#Ann#Anna#Annabal#Annabel#Annabell#Annabella#Annabelle#Annaliese#Annamaria#Annamarie#Anne#Annelise#Annemarie#Annetta#Annette#Anni#Annice#Annick#Annie#Annis#Annissa#Annmaria#Annora#Anny#Ansel#Ansley#Anson#Anthea#Anthony#Antoine#Antoinette#Antonetta#Antonia#Antonie#Antonietta#Antonina#Antony#Anwar#Anya#Aphrodite#Apollo#Appollo#Apria#April#Ara#Arabella#Aracely#Aram#Arch#Archer#Archibald#Archie#Ardath#Ardelle#Arden#Ardenia#Ardice#Ardis#Ardith#Ardyce#Ardys#Ardyth#Aretha#Ari#Aria#Ariana#Ariane#Aridatha#Ariel#Arien#Arin#Arissa#Arista#Aristotle#Arlen#Arlene#Arlo#Arlynda#Armand#Armanda#Armande#Armando#Armelle#Armina#Armstrong#Arnaud#Arne#Arnie#Arnold#Aron#Art#Artemis#Artemus#Arthur#Artie#Arty#Arvid#Arvin#Asa#Asabi#Ashby#Asher#Ashley#Ashling#Ashlyn#Asia#Asli#Asta#Aster#Asthore#Astin#Astra#Astrid#Atalanta#Athena#Auberta#Aubrey#Audrey#August#Augusta#Auguste#Augustina#Augustine#Augustus#Aura#Aure#Aurea#Aurel#Aurelea#Aurelia#Aurelie#Auria#Aurie#Aurilia#Aurora#Austin#Austina#Austine#Autumn#Ava#Avedis#Avel#Aveline#Averill#Avery#Avi#Avis#Aviva#Avram#Axel
#Baba#Baby#Bailey#Baird#Bairn#Bakula#Baldwin#Ballard#Bambi#Bancroft#Barak#Barb#Barbara#Barbie#Barclay#Bard#Barnaba#Barnabas#Barnaby#Barnardine#Barnett#Barney#Baron#Barone#Barr#Barran#Barret#Barretta#Barrie#Barrington#Barry#Bart#Barth#Bartholomew#Bartola#Barton#Bartram#Bary#Bash#Basil#Bast#Bastienne#Bathsheba#Baxter#Bayard#Baylee#Bayo#Bea#Beatrice#Beau#Beauregard#Becca#Beck#Becky#Bedros#Bee#Bel#Bela#Belay#Belden#Belind#Belinda#Bell#Bella#Belle#Bellini#Ben#Bena#Benard#Bendek#Benedick#Benedict#Benen#Benita#Benjamin#Benjammin#Bennett#Benny#Benson#Bentley#Benton#Berg#Berit#Berke#Berkeley#Bernadine#Bernard#Berne#Bernice#Bernie#Berny#Bert#Bertha#Bertram#Beryl#Bessie#Beth#Bethany#Betsy#Bette#Betty#Beulah#Bevan#Beverly#Bianca#Bibi#Bill#Billie#Billy#Bing#Birch#Birdie#Bjorn#Blaine#Blair#Blake#Blanche#Bliss#Blondelle#Blythe#Bo#Boaz#Bob#Bobbi#Bobbie#Bobby#Bogart#Bona#Bonaventure#Bond#Boniface#Bonita#Bonne#Bonnie#Bonny#Boone#Booth#Borden#Borg#Boris#Borka#Bowen#Bowie#Boyce#Boyd#Bracha#Brad#Braden#Bradford#Bradley#Brady#Braima#Bram#Bran#Brand#Branda#Brandie#Brandon#Brazil#Breanna#Breckin#Brede#Bree#Brein#Brend#Brenda#Brendan#Brenna#Brennan#Brent#Bret#Brett#Brewster#Brian#Briana#Brianna#Briar#Brice#Brick#Bridget#Brielle#Brier#Brigham#Bright#Brighton#Brigit#Brigitte#Brina#Brinley#Brinly#Brit#Brita#Britain#Britany#Britta#Brittania#Brock#Broderick#Brody#Bron#Brone#Bronson#Bronwen#Brook#Brooke#Brooklyn#Brooks#Bruce#Bruno#Bryan#Bryant#Bryce#Brygid#Brynn#Bryony#Bryton#Buck#Bud#Buddy#Buffy#Bunny#Burdette#Burgess#Burian#Burke#Burl#Burt#Burton#Butch#Butterfly#Buzz#Byron
#Cadence#Cady#Cael#Caesar#Cais#Caitlin#Caitlyn#Cal#Cala#Calandra#Calantha#Calder#Caldwell#Caleb#Calhoun#Calida#Calix#Calixte#Calla#Callia#Calliope#Calliste#Callum#Calvin#Calypso#Cambria#Cameron#Camilla#Camille#Camlin#Candace#Candice#Candida#Candra#Candy#Caprice#Cara#Carey#Cari#Carina#Carissa#Carita#Carl#Carla#Carleton#Carlie#Carlin#Carlos#Carlota#Carlotta#Carly#Carmel#Carmela#Carmelita#Carmen#Carmine#Carney#Carol#Carolena#Caroline#Carolyn#Caron#Carrie#Carroll#Carson#Carter#Carver#Cary#Cascata#Casey#Cashlin#Casimir#Casondra#Casper#Cassandra#Cassie#Cassius#Cater#Catherine#Cathi#Cathy#Cearo#Cece#Cecil#Cecilia#Cedric#Celeste#Celestyn#Celia#Celina#Celine#Cerise#Cesar#Chad#Chadwick#Chailyn#Chaim#Chaima#Chalmers#Chana#Chance#Chancella#Chanda#Chandler#Chandra#Channon#Chantal#Charis#Charisse#Charity#Charla#Charlee#Charlene#Charles#Charlotte#Charlton#Charmaine#Charo#Chars#Chase#Chastity#Chauncey#Chava#Cheche#Chelsa#Chelsea#Chelsia#Chen#Cher#Cherie#Cherry#Cheryl#Chester#Chet#Chevalier#Chic#Chick#Chico#Chilton#Chloe#Chloris#Chris#Chrissy#Christian#Christina#Christine#Christopher#Christophor#Christy#Chuck#Chyna#Cian#Ciara#Cicel#Cicely#Cicero#Ciel#Cinderella#Cindy#Cinnamon#Cira#Ciro#Cirocco#Cissi#Claire#Clara#Clare#Clarence#Clarissa#Clark#Clarke#Claude#Claudia#Clay#Clayborne#Clayland#Clayton#Clea#Cleanthe#Cleatus#Cleavant#Cleave#Clement#Clementine#Cleo#Cleon#Cletus#Cleveland#Cliff#Clifford#Clifton#Clint#Clio#Clive#Clover#Clyde#Cody#Colby#Cole#Colette#Colleen#Collin#Colm#Colman#Columb#Conley#Conner#Connie#Connley#Connor#Conrad#Conroy#Constance#Constantine#Consuelo#Content#Conway#Cooper#Copper#Cora#Corbett#Cordelia#Cordell#Coretta#Corey#Corin#Corine#Cornelius#Cornell#Cort#Cory#Cosette#Cosima#Cosmo#Coty#Courteney#Courtney#Craig#Crawford#Creda#Creighton#Crescent#Crissy#Cristina#Cristy#Crosby#Crystal#Cullen#Culver#Curran#Curt#Curtis#Cuthbert#Cybil#Cyd#Cynthia#Cyril#Cyrus
#Dacey#Dafydd#Dag#Dagmar#Dahila#Dahlia#Daisy#Dakota#Dale#Dalia#Dalila#Dalit#Dallas#Dalston#Dalton#Dalva#Damia#Damian#Damita#Damon#Damona#Dan#Dana#Dane#Dani#Danica#Daniel#Danielle#Danil#Dante#Danton#Danyl#Daphne#Dara#Darby#Darcy#Daria#Darius#Darla#Darlene#Darlita#Darnell#Darren#Darwin#Daryl#Dasan#Dasha#Dava#Davan#Dave#David#Davida#Davin#Davina#Davis#Davita#Davu#Daw#Dawn#Dayton#Dean#Deanna#Deanne#Debbi#Debbie#Debby#Deborah#Debra#Dee#Deidra#Deirdre#Dejah#Deka#Del#Delaine#Delaney#Delano#Delbert#Delfina#Delia#Delila#Delilah#Della#Delmore#Delores#Delta#Delu#Dembe#Demetria#Demetrius#Dena#Denali#Denis#Denise#Denna#Dennis#Denny#Denver#Deo#Deon#Derby#Derek#Derica#Derin#Dermot#Deron#Derora#Derrick#Derron#Derry#Des#Desana#Desdemona#Desi#Desiderio#Desiree#Desmond#Dessa#Deva#Devaki#Deven#Devi#Devin#Devlin#Devon#Devonna#Devorah#Devorit#Dewei#Dewey#Dewitt#Dexter#Dextra#Diallo#Diamond#Dian#Diana#Diane#Dianne#Diantha#Dianthe#Diata#Dick#Didier#Didrika#Diego#Dillan#Dillian#Dillon#Dima#Dina#Dinah#Dinh#Dino#Dinon#Dionne#Dionysius#Dionysus#Dior#Dirk#Dixie#Dixon#Dmitri#Doctor#Doda#Dodie#Doi#Dolan#Dolf#Dolly#Dolores#Dolph#Dom#Domani#Dominic#Dominick#Dominique#Dominy#Don#Donagh#Donal#Donald#Donat#Donato#Donelle#Donna#Donnel#Donnica#Donny#Donovan#Dooley#Dora#Dorah#Dorcas#Dore#Doreen#Dori#Doria#Dorian#Dorie#Dorinda#Doris#Dorit#Dorothea#Dorothy#Dorset#Dorsey#Dory#Dot#Dottie#Dotty#Doug#Dougal#Douglas#Dov#Doyle#Drake#Dreama#Drew#Dru#Drusilla#Duane#Duc#Dudley#Duena#Duff#Dugan#Duka#Duke#Dulce#Dulcea#Dulcie#Dulcina#Dulcinea#Dumi#Duncan#Dunixi#Dunja#Dunn#Dunne#Durant#Durward#Duscha#Dustin#Dusty#Dwayna#Dwayne#Dwight#Dyan#Dyani#Dyanne#Dylan#Dyllis#Dyre#Dysis
#Earl#Eartha#Easter#Ebenezer#Ebony#Ed#Eden#Edgar#Edie#Edith#Edmund#Edna#Edsel#Edward#Edwin#Efrem#Egan#Egil#Eiji#Eileen#Eilis#Eitan#Elaine#Elan#Elden#Eldon#Eldora#Eldridge#Eleanor#Eleazer#Electra#Elena#Elgin#Eli#Elias#Elijah#Elin#Elisabeth#Elise#Elissa#Elita#Eliza#Elizabeth#Elkan#Elke#Ella#Ellen#Ellena#Ellery#Ellia#Ellie#Elliot#Ellis#Ellison#Ellsworth#Elly#Elmer#Elmo#Eloise#Elroy#Elsa#Elsie#Elston#Elsy#Elton#Elvin#Elvira#Elvis#Elwin#Elwood#Elysia#Emanuel#Emanuele#Emele#Emera#Emerald#Emerson#Emery#Emil#Emilia#Emilie#Emilio#Emily#Emlyn#Emma#Emmanuel#Emmet#Emmly#Emory#Engelbert#Enid#Ennis#Enoch#Enos#Enrico#Enrique#Eolande#Ephraim#Epifanio#Er#Erasmus#Erhard#Eri#Eric#Erica#Erik#Erika#Erin#Erma#Ernestine#Erskine#Ervin#Erwin#Eryk#Esben#Eshe#Esma#Esme#Esmeralda#Estelle#Esther#Ethan#Ethel#Etta#Eudora#Eugene#Eugenia#Eulalie#Eunice#Euridice#Eurydice#Eustacia#Eva#Evan#Evane#Evangelia#Evangeline#Evania#Eve#Evelyn#Everett#Evonne#Eyal#Ezekiel#Ezra
#Fabian#Fairfax#Faith#Falkner#Fallon#Fanny#Farley#Farrah#Farrell#Fawn#Fay#Fedora#Felicia#Felix#Ferdinand#Fergus#Ferguson#Fern#Fernanado#Fernanda#Ferris#Ferrol#Fiachra#Fico#Fidel#Fidelio#Fidelity#Fidella#Field#Fielding#Fifi#Filbert#Filia#Filipina#Filmore#Fineen#Finlay#Finley#Finn#Finna#Finola#Fiona#Fionan#Fionn#Fionnula#Fiorenza#Fisk#Fisseha#Fitz#Fitzgerald#Flan#Flannery#Flavian#Fleming#Fleta#Fletcher#Fleur#Flint#Flora#Florenca#Florence#Floria#Florian#Floriane#Florida#Florrie#Flour#Flower#Floyd#Flynn#Forbes#Ford#Forest#Forrest#Forrester#Fortune#Foster#Fountain#Fox#Foy#Fran#France#Frances#Francesca#Francine#Francis#Francois#Francoise#Frank#Franklin#Frannie#Franza#Frasier#Frazer#Fred#Freda#Freddy#Frederica#Frederick#Freed#Freeman#Fremont#Freya#Frieda#Fritz#Fronde#Fruma#Fuller#Fulton#Fynn
#Gabby#Gabriel#Gabrielle#Gaetan#Gaetane#Gafna#Gage#Gail#Gaiya#Galdys#Gale#Galen#Gali#Galina#Gallagher#Gallia#Galvin#Gannon#Gardner#Gareth#Garfield#Garin#Garland#Garner#Garnet#Garrett#Garrick#Garrison#Garson#Garth#Garvey#Garvin#Garwood#Gary#Gaston#Gates#Gavan#Gavin#Gavivi#Gay#Gaye#Gayle#Gaylord#Gaynell#Gazali#Gazelle#Gazit#Gelsey#Gene#Genet#Geneva#Genevieve#Genna#Geno#George#Georgeanne#Georgia#Georgine#Geralding#Gerard#Gerffrey#Geri#Germain#Germaine#Gerri#Gerrie#Gerry#Gertrude#Gibson#Gideon#Gifford#Gigi#Gil#Gilbert#Gilberte#Gilda#Giles#Gili#Gillian#Gilon#Gin#Gina#Ginata#Ginger#Ginny#Gino#Giolla#Giona#Giovanni#Gisela#Giselle#Gita#Gitel#Gittel#Giuseppe#Giva#Giza#Gizi#Gladys#Glen#Glenda#Glendon#Glenn#Glenna#Glennis#Glora#Glori#Gloria#Glyn#Glynis#Glynn#Glynnis#Godana#Goddard#Godfrey#Golda#Goldie#Goldy#Gomer#Gonzalo#Gordon#Gordy#Gore#Grace#Gracie#Grady#Graham#Gram#Granger#Grania#Grant#Granville#Gratia#Grayson#Grazia#Greer#Greg#Gregor#Gregory#Greta#Gretchen#Griffin#Griffith#Griswold#Grove#Grover#Guido#Guillermo#Guinevere#Gunther#Gustave#Guthrie#Guy#Gwen#Gwendolyn#Gwyneth#Gypsy
#Hadley#Hakeem#Hal#Hale#Haley#Hall#Hallie#Halsey#Hamilton#Hamlet#Hamlin#Hampton#Hana#Hank#Hanley#Hanna#Hannah#Hannan#Hans#Happy#Harcourt#Hardy#Harlan#Harley#Harlow#Harmon#Harmony#Harold#Harper#Harriet#Harrison#Harry#Hartley#Harvey#Hashim#Haskel#Haslett#Hastings#Hattie#Hatty#Haven#Hayden#Hayes#Hayward#Haywood#Hazel#Heath#Heather#Hector#Hedda#Hedia#Hedva#Hedwig#Hedy#Hedya#Heidi#Helaine#Helen#Helena#Helene#Helga#Helia#Heller#Heloise#Henderson#Henri#Henrietta#Henry#Herbert#Herbst#Hercules#Herman#Hermione#Hernando#Herschell#Hershel#Hester#Hestia#Hewett#Hewitt#Heywood#Hidalgo#Hidi#Hila#Hilaire#Hilary#Hilda#Hilde#Hildegarde#Hillard#Hillary#Hillel#Hilliard#Hilton#Hinda#Hiram#Hiroko#Hirsi#Hobart#Hogan#Holden#Hollace#Holli#Hollis#Holly#Hollye#Holmes#Holt#Homer#Honey#Honor#Honora#Honoria#Hope#Horace#Horst#Hortense#Horton#Horus#Hosea#Howard#Howe#Howell#Howie#Hubert#Hue#Huela#Huey#Hugh#Hugo#Humbert#Humphrey#Hunt#Hunter#Huntington#Huntley#Hurley#Huso#Hussein#Hyacinth#Hyatt#Hyman
#Ian#Ianna#Ianthe#Ida#Idalee#Idalia#Idana#Idande#Idania#Iggy#Ignatius#Igor#Ilana#Ilario#Ilene#Iliana#Ilit#Ilithya#Illias#Ilse#Ilyssa#Imala#Imogene#Ina#Inari#Ince#India#Indira#Inez#Inga#Inge#Ingemar#Inger#Inglebert#Ingram#Ingrid#Innis#Iola#Ion#Iona#Ipo#Ira#Iram#Irene#Iria#Irina#Iris#Irma#Irving#Irwin#Isaac#Isabel#Isabella#Isabelle#Isabis#Isadora#Isaiah#Isha#Isi#Isidore#Isis#Ismail#Ismet#Isolde#Isra#Israel#Issay#Istas#Ita#Ivan#Ivana#Ivar#Ivory#Ivy
#LaDon#LaRue#LaWana#Lacey#Lachlan#Lacy#Laddie#Ladonna#Lael#Lahela#Laina#Lainey#Laird#Lajos#Lajuan#Lajuana#Lakin#Lala#Lalasa#Lale#Laleh#Lali#Lalita#Lalo#Lamar#Lamia#Lamis#Lamont#Lamya#Lan#Lana#Lanai#Lanaya#Lance#Lancelot#Landen#Landers#Landis#Landon#Landry#Lane#Lanelle#Lang#Langer#Langston#Lani#Lankston#Lanny#Lanza#Lara#Laraine#Larissa#Lark#Larry#Lars#Larvall#Larya#Lassie#Laszlo#Latham#Lathrop#Latimer#Latisha#Laura#Laurel#Lauren#Laurence#Laurent#Laurie#Laval#Lave#Laverne#Lavey#Lavi#Lavinia#Lavonne#Lawrence#Lawrencia#Lawton#Layne#Lazar#Lazarus#Lazzaro#Lea#Leah#Leal#Leander#Leandra#Leane#Leanne#Leavitt#Ledell#Lee#Leena#Leeto#Lefty#Lehana#Leif#Leigh#Leighton#Leila#Leilani#Lel#Leland#Lemuel#Lena#Lenore#Leo#Leon#Leona#Leonard#Leonora#Leontine#Leora#Leslie#Letitia#Levi#Lewis#Liam#Lian#Liana#Libby#Lida#Lila#Lillian#Lily#Lina#Lincoln#Linda#Lindsay#Lindsey#Linette#Linnea#Linus#Lionel#Lisa#Lisha#Lizhen#Llewellyn#Lloyd#Locke#Logan#Lois#Lola#Lolita#Lombard#Lon#Lona#London#Lonnie#Lonny#Lora#Lorelei#Lorelle#Loren#Loretta#Lori#Lorna#Lorne#Lorraine#Lottie#Lou#Louis#Louise#Lowell#Lucas#Lucia#Lucian#Lucille#Lucinda#Lucius#Lucy#Ludwig#Luella#Luis#Luke#Lulu#Lurleen#Lurlene#Luther#Lydia#Lyle#Lyman#Lyndon#Lynette#Lynn
#Jack#Jackie#Jackson#Jacob#Jacqueline#Jacques#Jade#Jaime#Jake#Jamal#James#Jamie#Jamil#Jan#Jana#Jane#Janelle#Janet#Janette#Jania#Janice#Janina#Janine#Janis#Janna#Jara#Jareb#Jared#Jarrett#Jarvis#Jasica#Jasmine#Jason#Jasper#Javen#Jay#Jayden#Jean#Jeanette#Jeanne#Jed#Jedidiah#Jeff#Jefferson#Jeffrey#Jemima#Jena#Jennifer#Jenny#Jeremiah#Jeremy#Jerome#Jerrie#Jerry#Jess#Jesse#Jessica#Jessie#Jethro#Jewel#Jill#Jillian#Jim#Jimmy#Jin#Jinny#Jira#Jiro#Jmi#Jo#Joachim#Joan#Joanna#Joanne#Job#Joby#Jocelyn#Jock#Jodi#Jody#Joe#Joel#Joelle#John#Johnna#Jolene#Jolie#Jon#Jonah#Jonas#Jonathan#Jordan#Jose#Joseph#Josephine#Josh#Joshua#Josiah#Josie#Joy#Joyce#Juan#Juanita#Judah#Judd#Judith#Judy#Jules#Julia#Julian#Julie#Julius#June#Justin#Justine
#Kailey#Kaili#Kairos#Kaitlyn#Kala#Kaley#Kali#Kalil#Kalila#Kalinda#Kalli#Kaloosh#Kame#Kameko#Kameryn#Kami#Kamil#Kamilah#Kana#Kane#Kanga#Kanoa#Kanya#Kaori#Kapila#Kara#Karan#Kare#Kareem#Karen#Karena#Kari#Karif#Karik#Karl#Karla#Kata#Kate#Katherine#Kathleen#Kathy#Katia#Katie#Katrina#Katungi#Katy#Kaula#Kay#Kaya#Kaycee#Kayee#Kayla#Keane#Kearney#Keefe#Keely#Keenan#Keir#Keith#Kelby#Kelila#Kelly#Kelsey#Kelvin#Ken#Kendall#Kendra#Kenn#Kennedy#Kenneth#Kent#Kenton#Kenyon#Kermit#Kerr#Kerry#Kerwin#Kesia#Kevin#Kevyn#Khali#Khalil#Kiah#Kiana#Kiandra#Kiara#Kibibe#Kiden#Kiele#Kieran#Kiersten#Kiet#Kiho#Kiki#Kiley#Killian#Kim#Kimball#Kimberly#Kimi#Kimmy#Kimo#Kin#Kina#Kincaid#Kineks#Kineta#Kinfe#King#Kingsley#Kingston#Kinipela#Kinsey#Kioko#Kiona#Kione#Kiora#Kip#Kipling#Kipp#Kira#Kirabo#Kiral#Kiran#Kirby#Kiri#Kiril#Kirk#Kiros#Kirra#Kirsi#Kirsten#Kisha#Kishi#Kit#Kita#Kitoko#Kitra#Kitty#Kiyoshi#Knox#Kora#Kris#Krishna#Kristen#Kristin#Kristina#Kristine#Kristopher#Krystal#Kurt#Kyle
#Mabel#Mac#MacKenzie#Madeline#Madge#Madison#Mae#Maggie#Mahalia#Maisie#Major#Malachi#Malcolm#Mallory#Malvin#Mamie#Mandel#Mandy#Manfred#Manuel#Mara#Marc#Marcel#Marcella#Marcia#Marcie#Marcus#Marcy#Margaret#Margery#Margo#Margot#Marguerite#Maria#Marian#Maribel#Marie#Mariel#Marietta#Marilyn#Marina#Mario#Marion#Maris#Marissa#Marjorie#Mark#Marlene#Marlo#Marlon#Marlow#Marnie#Marnin#Maro#Marrim#Marsha#Marshall#Marta#Martha#Martin#Martina#Marty#Marv#Marvel#Marvin#Mary#Maryann#Marybeth#Maryellen#Maryjo#Marylou#Masada#Mason#Massimo#Matana#Mate#Mateo#Mathan#Mathilda#Matia#Matilda#Matilde#Matrika#Matsu#Matt#Matta#Matteo#Matthew#Mattie#Matty#Maud#Maude#Maura#Maureen#Maurice#Mavis#Max#Maximilian#Maxine#Maxwell#May#Mayer#Maynard#Mckenna#Mea#Meachel#Meachell#Meachella#Mead#Meade#Meara#Meda#Medard#Medea#Meg#Megan#Megara#Meged#Mei#Meir#Meja#Mel#Melanie#Melantha#Melba#Mele#Meli#Melina#Melinda#Melissa#Melody#Melvin#Melvine#Melvyn#Mendel#Mercedes#Mercer#Mercy#Meredith#Meriel#Meris#Merle#Merlin#Merrick#Merrill#Merry#Mervin#Mervyn#Meryl#Meryle#Meyer#Mia#Micah#Michael#Michaela#Michal#Michelle#Mika#Mikaili#Mikasa#Mike#Mikhail#Mikkel#Mikko#Milan#Milandu#Mildred#Miles#Miley#Millard#Millicent#Millie#Mills#Milly#Milo#Milton#Mimi#Minda#Mindy#Minna#Minor#Mira#Miranda#Miriam#Mischa#Missy#Misty#Mitch#Mitchell#Mitzi#Modesty#Mohammed#Moira#Mollie#Molly#Mona#Monica#Monroe#Montague#Monte#Montgomery#Monty#Moore#Mordecai#Morey#Morgan#Morgana#Morley#Morris#Morse#Mort#Mortimer#Morton#Moselle#Moses#Moss#Muhammad#Murdock#Muriel#Murphy#Murray#Myra#Myrna#Myron#Myrtle
#Nacia#Nada#Nadia#Nadiera#Nalani#Nan#Nancy#Nanette#Nani#Naomi#Nara#Nari#Natalie#Natasha#Nathaniel#Neal#Neala#Necia#Nedra#Neelie#Neely#Neema#Neil#Nell#Nels#Nelson#Nerissa#Nero#Nessarose#Nessie#Nestor#Netis#Nettie#Netty#Neva#Neville#Nevin#Newton#Nicholas#Nick#Nicole#Niels#Nigel#Nike#Nikki#Nina#Nissa#Nita#Noah#Noaman#Noble#Noel#Noelani#Nola#Nolan#Noma#Nona#Nonnie#Nora#Norah#Norbert#Noreen#Norm#Norma#Norman#Normandy#Norris#Northrop#Norton#Nuncio#Nura#Nusair#Nydia#Nyssa
#Oafa#Oakes#Oakley#Obadiah#Odelene#Odeletta#Odelia#Odell#Odella#Odetta#Ogden#Olaf#Olga#Olin#Oliver#Olivia#Olympia#Omar#Omega#Ona#Onawa#Opal#Ophelia#Oren#Orion#Orland#Orlando#Orrin#Orson#Orville#Oscar#Osgood#Osmond#Oswald#Othella#Otis#Otto#Owen#Ozzie
#Pable#Pablo#Paco#Paddy#Padro#Page#Paige#Palma#Palmer#Paloma#Palti#Pamela#Pandora#Pansy#Paris#Park#Parker#Parnell#Parry#Parson#Pascal#Pascale#Pascha#Pasi#Pat#Patch#Patience#Patricia#Patricko#Patsy#Patti#Patton#Patty#Paul#Paula#Paulette#Pauline#Paxton#Peale#Pearl#Pearlie#Pearly#Pebbles#Pedro#Peers#Peggy#Pembroke#Penelopi#Penn#Penner#Penney#Penny#Pepper#Percival#Percy#Peregrine#Peri#Perlin#Perry#Pete#Peter#Petra#Petronella#Petunia#Peyton#Phaedra#Phil#Philip#Phillip#Phineas#Phoebe#Phyllis#Pia#Pierce#Pierre#Piper#Pippa#Pippy#Polly#Polo#Pomya#Ponce#Pony#Pooky#Poppy#Poria#Porter#Portia#Powell#Prentice#Prescott#Preston#Price#Priscilla#Prudence#Prue#Putnam
#Quella#Quentin#Querida#Quillan#Quin#Quincy#Quinlan#Quinn#Quinta#Quintin#Quinto#Quito
#Rabi#Rachel#Rad#Radcliffe#Rae#Rafael#Rafe#Rafferty#Rainer#Rainy#Raleigh#Ralph#Ram#Ramiro#Ramon#Ramona#Ramsay#Ramses#Rance#Rand#Randall#Randi#Randolph#Randy#Rane#Ranger#Rani#Rania#Ranit#Ransom#Raoul#Raphael#Raquel#Rasha#Raul#Raven#Ravi#Ray#Raymond#Raynor#Razi#Rea#Read#Reba#Rebecca#Red#Reda#Redell#Redford#Reece#Reed#Regan#Regina#Reginald#Reid#Reiko#Remington#Remus#Remy#Rena#Renata#Rene#Renee#Reta#Reuben#Rex#Reynard#Reynold#Rhea#Rhett#Rhona#Rhonda#Rhys#Riane#Rianna#Rianne#Rica#Rich#Richard#Richmond#Rick#Ricki#Rickie#Rico#Riley#Rin#Rina#Ring#Rio#Riordan#Rip#Ripley#Risa#Rita#Riva#River#Rivka#Roarke#Rob#Robert#Roberta#Robin#Robinson#Robyn#Rocco#Roch#Rochelle#Rochester#Rocia#Rock#Rockwell#Rocky#Rod#Roddy#Roderick#Rodger#Rodney#Roger#Roland#Rolf#Roman#Romeo#Ron#Rona#Ronald#Ronni#Rooney#Roosevelt#Rory#Rosalie#Rosalind#Roscoe#Rose#Rosemary#Ross#Roth#Rowena#Roxanne#Roy#Royal#Ruby#Rudd#Rudolph#Rudy#Rudyard#Rufus#Rupert#Russ#Russell#Rusty#Ruth#Ryan
#Sabina#Sabrina#Sadie#Salim#Sally#Salome#Salvatore#Sam#Samantha#Samara#Samson#Samuel#Sancho#Sanders#Sandra#Sandy#Sanford#Sapphire#Sargent#Sasha#Saul#Sawyer#Saxon#Sayer#Scarlett#Schuyler#Scott#Seamus#Sean#Sebastian#Selby#Selena#Selina#Selma#Serena#Serge#Seth#Seward#Seymour#Shaina#Shalom#Shamus#Shana#Shandy#Shane#Shani#Shannon#Shari#Sharon#Shaw#Shawn#Shea#Sheba#Sheena#Sheffield#Sheila#Shelby#Sheldon#Shelley#Shepherd#Sheridan#Sherlock#Sherman#Sherry#Sherwin#Sherwood#Sheryl#Shina#Shirley#Shoshana#Sid#Sidney#Sidonia#Sidonie#Sidra#Siegfried#Sigmund#Silas#Sileas#Silvain#Silvana#Silver#Silvester#Silvia#Simba#Simon#Simone#Sinclair#Siobhan#Skelly#Skip#Slade#Sloan#Smith#Solomon#Somerset#Sonia#Sonnie#Sonny#Sophie#Spencer#Stacy#Stanford#Stanislaus#Stanley#Stanton#Star#Stella#Stephanie#Stephen#Sterling#Sterne#Steve#Steven#Stewart#Stillman#Storm#Stuart#Sue#Sukey#Sullivan#Sumner#Sunny#Susan#Susanna#Susie#Sutherland#Sutton#Sven#Sydney#Sylvester#Sylvia
#Tab#Taban#Taber#Tabitha#Tacita#Tacy#Tad#Tadeo#Taffy#Tai#Taifa#Tailynn#Taima#Tait#Talbot#Talen#Talia#Taliesin#Taline#Talisa#Talisha#Tallys#Tam#Tama#Tamah#Tamara#Tamas#Tamasine#Tami#Tamika#Tammy#Tanner#Tansy#Tanya#Tarin#Tasha#Tasida#Tasmine#Tass#Tassos#Tasya#Tate#Tave#Tavi#Tavia#Tavita#Taylor#Tea#Ted#Teddy#Teenie#Teli#Telly#Telma#Templeton#Tenen#Teo#Terence#Teresa#Termon#Terra#Terran#Terrel#Terrence#Terrene#Terrill#Terris#Terry#Tess#Tessa#Tex#Thaddeus#Thady#Thane#Thatcher#Thea#Thel#Thelma#Thema#Themba#Theo#Theobald#Theodora#Theodore#Theodoric#Theresa#Therese#Theta#Thina#Thom#Thomas#Thor#Thora#Thorndike#Thornton#Thorpe#Thrine#Thurma#Thurson#Tiaret#Tierney#Tiffany#Tilda#Tilly#Tim#Timberly#Timothea#Timothy#Tina#Tisha#Titus#Tobias#Toby#Tod#Todd#Todo#Tolla#Tom#Tomas#Tommy#Toni#Tony#Topaz#Topaza#Topo#Topper#Tori#Torie#Torrance#Torrin#Torto#Tory#Toshi#Totie#Townsend#Tracy#Trapper#Trave#Travis#Trella#Tremain#Trent#Trevor#Trey#Tricia#Trilby#Trina#Trip#Tripp#Trisha#Trista#Tristan#Trixie#Trory#Troy#Trude#Trudy#Trulla#Trully#Truman#Tryne#Tucker#Tuesday#Tully#Tumo#Turner#Ty#Tyler#Tynan#Tyne#Tyrienne#Tyrone#Tyrus#Tyson
#Udell#Ugo#Ujana#Ula#Ulan#Ulani#Ulema#Ull#Ulla#Ulric#Ulysses#Uma#Umay#Umberto#Umeko#Umi#Ummi#Una#Unity#Upendo#Upton#Urania#Urbain#Urban#Uri#Uriel#Urilla#Urit#Ursa#Ursala#Ursula#Uta
#Vail#Val#Vala#Valarie#Valdemar#Valencia#Valentina#Valentine#Valeria#Valerie#Valiant#Valtina#Van#Vance#Vandalin#Vanessa#Vangie#Vanida#Vanna#Vanya#Varena#Vaughn#Vea#Veasna#Vedra#Vega#Velika#Velma#Velvet#Venedict#Venus#Vera#Verda#Vern#Verna#Vernados#Vernon#Veronica#Vesta#Vi#Vic#Vicki#Vicky#Victor#Victora#Victoria#Vida#Vidal#Vidor#Vienna#Vila#Vince#Vincent#Vine#Vinnie#Vinny#Vinson#Viola#Violet#Virgil#Virginia#Virote#Vitalis#Vito#Vitoria#Vittorio#Vivek#Vivi#Vivian#Viviana#Vivienne#Vlad#Vladimir#Volleny#Von#Vonda#Vondila#Vondra#Vui
#Wade#Wafa#Waggoner#Wainwright#Waite#Wakefield#Walden#Waldo#Walker#Wallace#Wallis#Wally#Walt#Walta#Walter#Walton#Wanda#Wander#Waneta#Ward#Warner#Warren#Washington#Waverly#Wayland#Wayne#Webb#Webster#Welby#Welcome#Wells#Wenda#Wendell#Wendi#Wendy#Werner#Wes#Wesley#Westbrook#Weston#Wheeler#Whitby#Whitfield#Whitley#Whitman#Whitney#Whittaker#Wilbur#Wilda#Wildon#Wiley#Wilford#Wilfred#Will#Willa#Willard#Willem#William#Willis#Willow#Wilma#Wilson#Wilton#Win#Winda#Winfield#Winnie
#Yancey#Yannik#Yeelves#Yolanda#Zebra#Zita".Split(Chr(35))
        Dim i As New Random()
        tbxUsername.Text = AllUser(i.Next(UBound(AllUser)))
    End Sub

    Private Sub TextBoxAvailableMem_KeyPress(ByVal sender As Object, ByVal e As KeyPressEventArgs) Handles TextBoxAvailableMem.KeyPress
        If Char.IsDigit(e.KeyChar) Or e.KeyChar = Chr(8) Then
            e.Handled = False
        Else
            e.Handled = True
        End If
    End Sub

    Private Sub LabelUsername_Click(sender As Object, e As EventArgs) Handles LabelUsername.Click

    End Sub

    Private Sub tbxUsername_Click(sender As Object, e As EventArgs) Handles tbxUsername.Click

    End Sub

    Private Sub LabelServerPlayerCount_Click(sender As Object, e As EventArgs) Handles LabelServerPlayerCount.Click

    End Sub

    Private Sub LabelServerVersion_Click(sender As Object, e As EventArgs) Handles LabelServerVersion.Click

    End Sub

    Private Sub MetroTabPage2_Click(sender As Object, e As EventArgs) Handles MetroTabPage2.Click

    End Sub
End Class


