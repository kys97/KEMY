using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Firebase.Database;

public enum UserProfileLoadStatus { Found, Missing, Failed }

public sealed class UserProfileLoadResult
{
    public UserProfileLoadStatus Status { get; private set; }
    public DataClass Data { get; private set; }
    public Exception Error { get; private set; }

    private UserProfileLoadResult(UserProfileLoadStatus status, DataClass data, Exception error)
    {
        Status = status;
        Data = data;
        Error = error;
    }

    public static UserProfileLoadResult Found(DataClass data) => new UserProfileLoadResult(UserProfileLoadStatus.Found, data, null);
    public static UserProfileLoadResult Missing() => new UserProfileLoadResult(UserProfileLoadStatus.Missing, null, null);
    public static UserProfileLoadResult Failed(Exception error) => new UserProfileLoadResult(UserProfileLoadStatus.Failed, null, error);
}

public sealed class FirebaseAccountService
{
    public FirebaseAuth Auth { get; private set; }
    public DatabaseReference UserDatabase { get; private set; }
    public bool IsReady { get; private set; }
    public FirebaseUser CurrentUser => IsReady ? Auth.CurrentUser : null;
    public AccountServerClient AccountServer { get; private set; }

    public async Task InitializeAsync()
    {
        if (IsReady) return;
        DependencyStatus status = await FirebaseApp.CheckAndFixDependenciesAsync();
        if (status != DependencyStatus.Available)
            throw new InvalidOperationException("Firebase dependencies are unavailable: " + status);
        Auth = FirebaseAuth.DefaultInstance;
        UserDatabase = FirebaseDatabase.DefaultInstance.GetReference("UserInfo");
        IsReady = true;
    }

    public void ConfigureAccountServer(string baseUrl) { AccountServer = new AccountServerClient(baseUrl); }
    public Task<AuthResult> SignInAsync(string email, string password) { EnsureReady(); return Auth.SignInWithEmailAndPasswordAsync(email, password); }
    public Task<AuthResult> SignInWithCustomTokenAsync(string customToken) { EnsureReady(); return Auth.SignInWithCustomTokenAsync(customToken); }
    public Task SendPasswordResetEmailAsync(string email) { EnsureReady(); return Auth.SendPasswordResetEmailAsync(email); }
    public void SignOut() { EnsureReady(); Auth.SignOut(); }

    public async Task<UserProfileLoadResult> LoadProfileAsync(string uid)
    {
        EnsureReady();
        try
        {
            DataSnapshot snapshot = await UserDatabase.Child(uid).GetValueAsync();
            if (!snapshot.Exists) return UserProfileLoadResult.Missing();

            DataClass data = new DataClass();
            data.info.uid = uid;
            data.info.login = true;
            data.info.email = CurrentUser != null ? CurrentUser.Email : null;
            ReadString(snapshot, "loginId", value => data.info.loginId = value);
            ReadString(snapshot, "name", value => data.info.name = value);
            ReadInt(snapshot, "coin", value => data.info.coin = value);
            ReadInt(snapshot, "heart", value => data.info.heart = value);
            ReadString(snapshot, "skinSet", value => data.avatar_info.skin = value);
            ReadString(snapshot, "faceSet", value => data.avatar_info.face = value);
            ReadString(snapshot, "skinList", value => data.inven.skin = SplitList(value));
            ReadString(snapshot, "faceList", value => data.inven.face = SplitList(value));
            return UserProfileLoadResult.Found(data);
        }
        catch (Exception exception) { return UserProfileLoadResult.Failed(exception); }
    }

    public async Task<bool> CreateProfileIfMissingAsync(string uid, DataClass data)
    {
        EnsureReady();
        DataSnapshot existing = await UserDatabase.Child(uid).GetValueAsync();
        if (existing.Exists) return false;
        await UserDatabase.Child(uid).UpdateChildrenAsync(ToDictionary(data));
        return true;
    }

    public Task SaveProfileAsync(string uid, DataClass data)
    {
        EnsureReady();
        return UserDatabase.Child(uid).UpdateChildrenAsync(ToDictionary(data));
    }

    private static Dictionary<string, object> ToDictionary(DataClass data)
    {
        return new Dictionary<string, object>
        {
            { "name", data.info.name }, { "coin", data.info.coin }, { "heart", data.info.heart },
            { "skinSet", data.avatar_info.skin }, { "faceSet", data.avatar_info.face },
            { "skinList", string.Join(",", data.inven.skin) }, { "faceList", string.Join(",", data.inven.face) }
        };
    }

    private static void ReadString(DataSnapshot snapshot, string key, Action<string> assign)
    {
        DataSnapshot child = snapshot.Child(key);
        if (child.Exists && child.Value != null) assign(child.Value.ToString());
    }

    private static void ReadInt(DataSnapshot snapshot, string key, Action<int> assign)
    {
        DataSnapshot child = snapshot.Child(key); int value;
        if (child.Exists && child.Value != null && int.TryParse(child.Value.ToString(), out value)) assign(value);
    }

    private static List<string> SplitList(string value) => string.IsNullOrEmpty(value) ? new List<string>() : new List<string>(value.Split(','));
    private void EnsureReady() { if (!IsReady) throw new InvalidOperationException("Firebase has not finished initializing."); }
}
