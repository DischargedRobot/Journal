import { Header } from "@/widgets/header"
import { SidePanelPersonalMenu } from "@/widgets/side-panel-personal-menu"

const PersonalLayout = ({ children }: { children: React.ReactNode }) => {
    return (
        <>
            <Header />
            <main className="flex my-0 mx-auto w-full h-full overflow-auto">
                <SidePanelPersonalMenu />
                <div className="flex flex-col p-6">{children}</div>
            </main>
        </>
    )
}

export default PersonalLayout;