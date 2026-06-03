import { Header } from "@/widgets/header"
import { SidePanelPersonalMenu } from "@/widgets/side-panel-personal-menu"

const PersonalLayout = ({ children }: { children: React.ReactNode }) => {
    return (
        <div className="flex flex-col h-full w-full">
            <Header />
            <main className="flex my-0 mx-auto w-full h-full overflow-auto">
                <SidePanelPersonalMenu />
                <div className="flex w-full">{children}</div>
            </main>
        </div>
    )
}

export default PersonalLayout;