import { Header } from "@/widgets/header"
import { BackPanelPersonalMenu } from "@/widgets/back-panel-personal-menu"

const PersonalLayout = ({ children }: { children: React.ReactNode }) => {
    return (
        <>
            <Header />
            <main className="flex my-0 mx-auto w-full h-full overflow-auto">
                <BackPanelPersonalMenu />
                <div className="flex flex-col p-6">{children}</div>
            </main>
        </>
    )
}

export default PersonalLayout;